﻿using Data.Entities.Discord;
using Data.Entities.Tags;
using Data.Projections;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries;

public static class TagQueries
{
    /// <summary>
    /// Searches <see cref="Tag"/>s visible in guild with <paramref name="guildId"/>.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public static IQueryable<Tag> VisibleIn(this IQueryable<Tag> query, Snowflake guildId) => query
        .Where(x => x.GuildId == null || x.GuildId == guildId);

    /// <summary>
    /// Searches for <see cref="Tag"/>s visible in <paramref name="guild"/>.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="guild"></param>
    /// <returns></returns>
    public static IQueryable<Tag> VisibleIn(this IQueryable<Tag> query, DiscordGuild guild) => query
        .VisibleIn(guild.Id);

    /// <summary>
    /// Searches for <see cref="Tag"/>s that have name similar to <see cref="prompt"/>.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public static IQueryable<Tag> SearchByName(this IQueryable<Tag> query, string prompt) => query
        .Where(x => EF.Functions.ILike(x.Name, $"%{prompt}%"));

    /// <summary>
    /// Includes data necessary to access <see cref="Tag.Text"/>.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static IQueryable<Tag> WithText(this IQueryable<Tag> query) => query
        .Include(x => ((AliasTag)x).ReferencedTag);

    /// <summary>
    /// Projects <see cref="Tag"/>s into <see cref="TagOverview"/>.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static IQueryable<TagOverview> AsOverviews(this IQueryable<Tag> query) => query
        .Select(x => new TagOverview
        {
            Id = x.Id,
            Name = x.Name,
            GuildId = x.GuildId,
            OwnerId = x.OwnerId,
            TagKind = x is MessageTag
                ? TagKind.Message
                : TagKind.Alias
        });
}