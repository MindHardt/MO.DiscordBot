using Disqord;

namespace Data.Projections;

public record TagOverview
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required Snowflake? GuildId { get; set; }
    public required Snowflake? OwnerId { get; set; }
    public required TagKind TagKind { get; set; }
}