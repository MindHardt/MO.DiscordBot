using Disqord;

namespace Bot;

/// <summary>
/// Contains various extension methods for bot and UI.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Creates <see cref="LocalEmoji"/> from <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LocalEmoji ToEmoji(this bool value)
        => LocalEmoji.Unicode(value ? "✅" : "❌");
}