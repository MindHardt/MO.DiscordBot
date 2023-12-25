using Disqord;
using Disqord.Bot.Commands.Application;
using Disqord.Gateway;
using Qommon;

namespace Bot;

/// <summary>
/// Contains various extension methods for bot and UI.
/// </summary>
public static class DiscordExtensions
{
    /// <summary>
    /// Creates <see cref="LocalEmoji"/> from <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LocalEmoji ToEmoji(this bool value)
        => LocalEmoji.Unicode(value ? "✅" : "❌");

    /// <summary>
    /// Formats this <see cref="DateTimeOffset"/> as a discord timestamp.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static string ToDiscordTimestamp(this DateTimeOffset time, TimestampKind kind = TimestampKind.Relative)
        => $"<t:{time.ToUnixTimeSeconds()}:{(char)kind}>";

    public enum TimestampKind : ushort
    {
        /// <summary>
        /// The <see cref="TimestampKind"/> displayed as <c>d</c>.
        /// </summary>
        /// <example>10.11.2023</example>
        ShortDate = 'd',
        /// <summary>
        /// The <see cref="TimestampKind"/> displayed as <c>D</c>.
        /// </summary>
        /// <example>10 ноября 2023 г.</example>
        LongDate = 'D',
        /// <summary>
        /// The <see cref="TimestampKind"/> displayed as <c>t</c>.
        /// </summary>
        /// <example>21:11</example>
        ShortTime = 't',
        /// <summary>
        /// The <see cref="TimestampKind"/> displayed as <c>T</c>.
        /// </summary>
        /// <example>21:11:00</example>
        LongTime = 'T',
        /// <summary>
        /// The <see cref="TimestampKind"/> displayed as <c>f</c>.
        /// </summary>
        /// <example>10 ноября 2023 г., 21:11</example>
        ShortDateTime = 'f',
        /// <summary>
        /// The <see cref="TimestampKind"/> displayed as <c>F</c>.
        /// </summary>
        /// <example>пятница, 10 ноября 2023 г., 21:11</example>
        LongDateTime = 'F',
        /// <summary>
        /// The <see cref="TimestampKind"/> displayed as <c>R</c>.
        /// </summary>
        /// <example>4 минуты назад</example>
        Relative = 'R'
    }

    public static string GetLink(this IMessage message) => message is IGatewayUserMessage { GuildId: not null } gatewayMsg
        ? $"https://discord.com/channels/{gatewayMsg.GuildId.Value}/{gatewayMsg.ChannelId}/{gatewayMsg.Id}"
        : $"https://discord.com/channels/@me/{message.ChannelId}/{message.Id}";

    public static string GetChannelLink(this IDiscordApplicationGuildCommandContext context) =>
        $"https://discord.com/channels/{context.GuildId}/{context.ChannelId}";

    public static LocalMessage CreateQuote(this IMessage message) => new()
    {
        Embeds = new LocalEmbed[]
        {
            new()
            {
                Color = Color.DarkGoldenrod,
                Author = new LocalEmbedAuthor
                {
                    IconUrl = message.Author.GetAvatarUrl(),
                    Name = message.Author.Name
                },
                ImageUrl = Optional.FromNullable((message as IUserMessage)?.Attachments.FirstOrDefault()?.Url),
                Description = message.Content,
                Timestamp = DateTimeOffset.Now,
                Fields = new List<LocalEmbedField>()
                {
                    new()
                    {
                        Name = Markdown.Bold(Resources.Quote_PostedIn),
                        Value = message.GetLink(),
                        IsInline = false
                    }
                }
            }
        }
    };
}