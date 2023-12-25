using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Bogus;
using Data;
using Data.Entities.Discord;
using Data.Entities.Tags;
using Data.Queries;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Discord.Tags;

/// <summary>
/// A service that can validate <see cref="Tag"/> names,
/// <see cref="DiscordGuild.TagPrefix"/>es
/// and finding tag names in messages.
/// </summary>
/// <param name="memoryCache"></param>
public partial class TagService(
    DataContext dataContext,
    IMemoryCache memoryCache,
    Faker faker,
    ILogger<TagService> logger)
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string TagNameAllowedCharacters = @"[\p{L}\d_\.,\-]";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string TagNameRegexString = $"^{TagNameAllowedCharacters}+$";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string GuildPrefixRegexString = ".+";

    [GeneratedRegex(TagNameRegexString)]
    private partial Regex TagNameRegex();

    [GeneratedRegex(GuildPrefixRegexString)]
    private partial Regex GuildPrefixRegex();

    /// <summary>
    /// Gets a tag with name equal to <paramref name="name"/>.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="name"></param>
    /// <param name="ct"></param>
    /// <returns>
    /// The found <see cref="Tag"/> or <see langword="null"/>
    /// if there are no tags or the match is ambiguous.
    /// </returns>
    public async Task<Tag?> FindExactAsync(
        Snowflake guildId,
        string name,
        CancellationToken ct = default)
    {
        var tag = await dataContext.Tags
            .IncludeReferencedTag()
            .WhereVisibleIn(guildId)
            .WhereNameExactly(name)
            .FirstOrDefaultAsync(ct);

        logger.LogInformation("Looking up tag with name {Name} in guild {Guild}, found {Result}",
            name, guildId, tag?.Name);

        return tag;
    }

    /// <summary>
    /// Validates <see cref="Tag"/> name.
    /// </summary>
    /// <param name="tagName"></param>
    /// <exception cref="ArgumentException">If <paramref name="tagName"/> is not a valid tag name.</exception>
    public void ValidateTagName(string tagName)
    {
        if (TagNameValid(tagName) is false)
            TagThrows.ThrowTagNameInvalid(tagName);
    }

    /// <summary>
    /// Validates <see cref="Tag"/> name.
    /// </summary>
    /// <param name="tagName"></param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="tagName"/> is valid, otherwise <see langword="false"/>.
    /// </returns>
    public bool TagNameValid(string tagName)
        => tagName is { Length: <= Tag.MaxNameLength } && TagNameRegex().IsMatch(tagName);

    /// <summary>
    /// Validates <see cref="DiscordGuild.TagPrefix"/>.
    /// </summary>
    /// <param name="prefix"></param>
    /// <exception cref="ArgumentException">If <paramref name="prefix"/> is not a valid guild prefix.</exception>
    public void ValidateGuildPrefix(string prefix)
    {
        if (GuildPrefixValid(prefix) is false)
            TagThrows.ThrowTagPrefixInvalid(prefix);
    }

    /// <summary>
    /// Validates <see cref="DiscordGuild.TagPrefix"/>.
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="prefix"/> is valid, otherwise <see langword="false"/>.
    /// </returns>
    public bool GuildPrefixValid(string prefix)
        => prefix is { Length: <= DiscordGuild.MaxTagPrefixLength } && GuildPrefixRegex().IsMatch(prefix);

    /// <summary>
    /// Finds tag name in <see cref="message"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public string? FindTagName(string message, string prefix)
    {
        var cacheKey = $"{nameof(DiscordGuild.TagPrefix)}_{prefix}";
        var regex = memoryCache.GetOrCreate(cacheKey, _ => CreateRegex(prefix))!;
        var match = regex.Match(message);

        var foundTagName = match.Groups["NAME"].Value is { Length: > 0 } tagName
            ? tagName
            : null;

        logger.LogInformation("Looking for tag name in text {Text} with prefix {Prefix}: found {Tag}",
            message, prefix, foundTagName);

        return foundTagName;
    }

    /// <summary>
    /// Generates random tag name.
    /// </summary>
    /// <returns></returns>
    public string GenerateRandomTagName()
    {
        int attempt = 1;
        while (true)
        {
            var tagName = string.Join('-', faker.Lorem.Words()).ToLower();
            if (TagNameValid(tagName))
            {
                logger.LogInformation("Generated random tag name {Name} in {Attempts} attempts",
                    tagName, attempt);
                return tagName;
            }
            attempt++;
        }
    }

    private Regex CreateRegex(string prefix)
    {
        var pattern = @$"{Regex.Escape(prefix)}\s?(?<NAME>{TagNameAllowedCharacters}{{0,{Tag.MaxNameLength}}})";
        logger.LogInformation("Creating tag lookup regex with pattern {Pattern}", pattern);

        return new Regex(pattern, RegexOptions.Compiled);
    }
}