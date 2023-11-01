namespace Data.Entities.Discord;

public static class DiscordEntitiesExtensions
{
    /// <summary>
    /// Formats this <see cref="DiscordUser.AccessLevel"/> as a fancy localized string.
    /// </summary>
    /// <param name="accessLevel"></param>
    /// <returns></returns>
    public static string ToName(this DiscordUser.AccessLevel accessLevel)
    {
        var name = accessLevel switch
        {
            DiscordUser.AccessLevel.Default => AccessLevelNames.Default,
            DiscordUser.AccessLevel.Advanced => AccessLevelNames.Advanced,
            DiscordUser.AccessLevel.Helper => AccessLevelNames.Helper,
            DiscordUser.AccessLevel.Administrator => AccessLevelNames.Admin,
            _ => AccessLevelNames.Unknown
        };
        return $"{name} ({(byte)accessLevel})";
    }
}