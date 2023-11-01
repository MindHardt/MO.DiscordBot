using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Bogus;
using Data.Entities.Discord;
using Data.Entities.Tags;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Discord.Tags;

/// <summary>
/// A service that can validate <see cref="Tag"/> names,
/// <see cref="DiscordGuild.TagPrefix"/>es
/// and finding tag names in messages.
/// </summary>
/// <param name="memoryCache"></param>
public partial class TagNameService(IMemoryCache memoryCache, Faker faker)
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string TagNameAllowedCharacters = @"[\p{L}\d_\.,\-]";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string TagNameRegexString = $"^{TagNameAllowedCharacters}+$";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string GuildPrefixRegexString = "[\\S]+";

    [GeneratedRegex(TagNameRegexString)]
    private partial Regex TagNameRegex();

    [GeneratedRegex(GuildPrefixRegexString)]
    private partial Regex GuildPrefixRegex();

    /// <summary>
    /// Validates <see cref="Tag"/> name.
    /// </summary>
    /// <param name="tagName"></param>
    /// <exception cref="ArgumentException">If <paramref name="tagName"/> is not a valid tag name.</exception>
    public void ValidateTagName(string tagName)
    {
        if (TagNameValid(tagName) is false)
            throw new ArgumentException(string.Format(TagResources.TagNameInvalidErrorMessage, tagName));
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
            throw new ArgumentException(string.Format(TagResources.GuildPrefixInvalidErrorMessage, prefix));
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

        return match.Groups["NAME"].Value is { Length: > 0 } tagName
            ? tagName
            : null;
    }

    /// <summary>
    /// Generates random tag name.
    /// </summary>
    /// <returns></returns>
    public string GenerateRandomTagName()
    {
        while (true)
        {
            var tagName = string.Join('-', faker.Lorem.Words()).ToLower();
            if (TagNameValid(tagName))
            {
                return tagName;
            }
        }
    }

    private static Regex CreateRegex(string prefix) =>
        new($"{Regex.Escape(prefix)}(?<NAME>{TagNameAllowedCharacters}{{0,{Tag.MaxNameLength}}})", RegexOptions.Compiled);
}