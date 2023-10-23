using Data.Entities.Discord;
using Data.Entities.Tags;

namespace Services.Tags;

/// <summary>
/// A class used to create all types of <see cref="Tag"/>s.
/// </summary>
public class TagFactory
{
    /// <summary>
    /// Creates a new <see cref="MessageTag"/> from provided data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="content"></param>
    /// <param name="owner"></param>
    /// <param name="guild"></param>
    /// <returns></returns>
    public virtual MessageTag CreateMessageTag(
        string name,
        string content,
        DiscordUser? owner,
        DiscordGuild? guild) => new()
    {
        Name = name,
        Content = content,
        GuildId = guild?.Id,
        Guild = guild,
        OwnerId = owner?.Id,
        Owner = owner
    };

    /// <summary>
    /// Creates a new <see cref="AliasTag"/> from provided data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="referencedTag"></param>
    /// <param name="owner"></param>
    /// <param name="guild"></param>
    /// <returns></returns>
    public virtual AliasTag CreateAliasTag(
        string name,
        MessageTag referencedTag,
        DiscordUser? owner,
        DiscordGuild? guild) => new()
    {
        Name = name,
        GuildId = guild?.Id,
        Guild = guild,
        OwnerId = owner?.Id,
        Owner = owner,
        ReferencedTagId = referencedTag.Id,
        ReferencedTag = referencedTag
    };
}