using System.Diagnostics.CodeAnalysis;

namespace Application.Discord.Tags;

public static class TagThrows
{
    [DoesNotReturn]
    public static void ThrowTagNotFound(string tagName)
        => throw new ArgumentException(string.Format(TagResources.TagNotFoundErrorMessage, tagName));

    [DoesNotReturn]
    public static void ThrowTagPrefixInvalid(string prefix)
        => throw new ArgumentException(string.Format(TagResources.GuildPrefixInvalidErrorMessage, prefix));

    [DoesNotReturn]
    public static void ThrowTagNameInvalid(string tagName)
        => throw new ArgumentException(string.Format(TagResources.TagNameInvalidErrorMessage, tagName));

    [DoesNotReturn]
    public static void ThrowTagNameOccupied(string tagName)
        => throw new AggregateException(string.Format(TagResources.TagNameOccupiedErrorMessage, tagName));

    [DoesNotReturn]
    public static void ThrowAccessDenied()
        => CommonThrows.ThrowAccessDenied(TagResources.TagEditOrDeleteAction);
}