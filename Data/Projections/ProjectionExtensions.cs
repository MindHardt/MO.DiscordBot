using Data.Entities.Tags;

namespace Data.Projections;

public static class ProjectionExtensions
{
    public static TagKind GetKind(this Tag tag) => tag switch
    {
        MessageTag => TagKind.Message,
        AliasTag => TagKind.Alias,
        _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, null)
    };
}