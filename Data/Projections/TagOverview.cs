using Disqord;

namespace Data.Projections;

public record TagOverview(int Id, string Name, Snowflake? GuildId, Snowflake? OwnerId, TagKind TagKind);