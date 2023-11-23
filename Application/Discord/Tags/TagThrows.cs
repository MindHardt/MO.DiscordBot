using System.Diagnostics.CodeAnalysis;

namespace Application.Discord.Tags;

public static class TagThrows
{
    /// <inheritdoc cref="TagResources.TagNotFoundErrorMessage"/>
    [DoesNotReturn]
    public static void ThrowTagNotFound(string tagName)
        => throw new ArgumentException(string.Format(TagResources.TagNotFoundErrorMessage, tagName));

    /// <inheritdoc cref="TagResources.GuildPrefixInvalidErrorMessage"/>
    [DoesNotReturn]
    public static void ThrowTagPrefixInvalid(string prefix)
        => throw new ArgumentException(string.Format(TagResources.GuildPrefixInvalidErrorMessage, prefix));

    /// <inheritdoc cref="TagResources.TagNameInvalidErrorMessage"/>
    [DoesNotReturn]
    public static void ThrowTagNameInvalid(string tagName)
        => throw new ArgumentException(string.Format(TagResources.TagNameInvalidErrorMessage, tagName));

    /// <inheritdoc cref="TagResources.TagNameOccupiedErrorMessage"/>
    [DoesNotReturn]
    public static void ThrowTagNameOccupied(string tagName)
        => throw new AggregateException(string.Format(TagResources.TagNameOccupiedErrorMessage, tagName));

    /// <inheritdoc cref="CommonThrows.ThrowAccessDenied"/>
    /// <inheritdoc cref="TagResources.TagEditOrDeleteAction"/>
    [DoesNotReturn]
    public static void ThrowAccessDenied()
        => CommonThrows.ThrowAccessDenied(TagResources.TagEditOrDeleteAction);
}