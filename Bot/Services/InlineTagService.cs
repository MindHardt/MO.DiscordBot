using Application;
using Application.Accessors;
using Application.Discord.Tags;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Disqord.Rest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bot.Services;

public class InlineTagService(ILogger<InlineTagService> logger) : DiscordBotService
{
    private static readonly LocalEmoji NotFoundEmoji = LocalEmoji.Unicode("❔");

    protected override ValueTask OnReady(ReadyEventArgs e)
    {
        logger.LogDebug("Service initialized");
        return ValueTask.CompletedTask;
    }

    protected override async ValueTask OnMessageReceived(BotMessageReceivedEventArgs e)
    {
        if (e.GuildId is null || Bot.GetUser(e.AuthorId)?.IsBot is not false)
        {
            return;
        }

        await using var scope = Bot.Services.CreateAsyncScope();

        var guildAccessor = scope.ServiceProvider.GetRequiredService<DiscordGuildAccessor>();
        var guild = await guildAccessor.GetAsync(e.GuildId.Value);
        if (guild is not { InlineTagsEnabled: true })
        {
            return;
        }

        var tagNameService = scope.ServiceProvider.GetRequiredService<TagNameService>();
        var tagName = tagNameService.FindTagName(e.Message.Content, guild.TagPrefix);
        if (tagName is null)
        {
            return;
        }

        var getTagRequest = new GetTagRequest(e.GuildId.Value, tagName);
        var tagResult = await scope.ServiceProvider
            .GetRequiredService<GetTagHandler>()
            .HandleAsync(getTagRequest, CancellationToken.None)
            .AsResult();

        if (tagResult.Success is false)
        {
            await Bot.AddReactionAsync(e.ChannelId, e.MessageId, NotFoundEmoji);
            return;
        }

        var response = new LocalMessage()
            .WithContent(tagResult.Value.Text)
            .WithReply(e.MessageId)
            .WithAllowedMentions(LocalAllowedMentions.None);

        await Bot.SendMessageAsync(e.ChannelId, response);
        await base.OnMessageReceived(e);
    }
}