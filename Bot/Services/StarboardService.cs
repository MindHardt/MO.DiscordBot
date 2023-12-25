using Data;
using Data.Entities.Starboard;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Disqord.Rest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Qommon;

namespace Bot.Services;

public class StarboardService(ILogger<StarboardService> logger) : DiscordBotService
{
    protected override async ValueTask OnReactionAdded(ReactionAddedEventArgs e)
    {
        if (e.GuildId is null)
        {
            return;
        }

        var originalMessage = e.Message ?? await Bot.FetchMessageAsync(e.ChannelId, e.MessageId);
        if (originalMessage!.Reactions is not { HasValue: true, Value: var reactions })
        {
            return;
        }

        await using var scope = Bot.Services.CreateAsyncScope();

        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var emojiName = LocalEmoji.FromEmoji(e.Emoji)!.GetReactionFormat();

        var track = await dataContext.StarboardTracks
            .Include(x => x.DiscordGuild)
            .FirstOrDefaultAsync(x => x.Emoji == emojiName && x.GuildId == e.GuildId.Value);

        if (track?.DiscordGuild.StarboardChannelId is null)
        {
            return;
        }
        if (reactions.TryGetValue(e.Emoji, out var reaction) is false || reaction.Count < track.StarboardThreshold)
        {
            return;
        }

        var quoteMessage = CreateQuoteMessage(originalMessage, reaction)
            .WithContent(string.Join(", ", reactions.Values
                .OrderByDescending(x => x.Count)
                .Take(Discord.Limits.Message.Embed.MaxFieldAmount)
                .Select(x => $"{Markdown.Bold(x.Count)} {x.Emoji}")));

        var starboardQuote = await dataContext.StarboardQuotes
            .FirstOrDefaultAsync(x =>
                x.DiscordGuildId == track.GuildId &&
                x.OriginalMessageId == e.MessageId);

        if (starboardQuote is null)
        {
            var starboardMessage = await Bot.SendMessageAsync(track.DiscordGuild.StarboardChannelId.Value, quoteMessage);
            starboardQuote ??= CreateDatabaseQuote(originalMessage, starboardMessage, e.GuildId.Value);

            dataContext.Add(starboardQuote);
            await dataContext.SaveChangesAsync();

            logger.LogInformation("Creating new starboard quote from message {Message} in guild {Guild}",
                starboardQuote.OriginalMessageId, starboardQuote.DiscordGuildId);
        }
        else
        {
            await Bot.ModifyMessageAsync(
                track.DiscordGuild.StarboardChannelId.Value,
                starboardQuote.QuoteMessageId,
                starboardMessage =>
                {
                    starboardMessage.Content = quoteMessage.Content;
                    starboardMessage.Embeds = Optional.Convert(quoteMessage.Embeds, list => list.AsEnumerable());
                });

            logger.LogInformation("Updated starboard quote in message {Message} of guild {Guild}",
                starboardQuote.QuoteMessageId, starboardQuote.DiscordGuildId);
        }

        await base.OnReactionAdded(e);
    }

    private static LocalMessage CreateQuoteMessage(IMessage message, IMessageReaction reaction) =>
        message.CreateQuote().WithContent($"{Markdown.Bold(reaction.Count)} {reaction.Emoji}");

    private static StarboardQuote CreateDatabaseQuote(IMessage originalMessage, IMessage starboardMessage, Snowflake guildId) => new()
    {
        OriginalMessageId = originalMessage.Id,
        DiscordGuildId = guildId,
        QuoteMessageId = starboardMessage.Id,
    };
}