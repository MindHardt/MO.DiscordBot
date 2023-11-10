using Disqord;

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
}