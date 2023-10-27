using Data;
using Data.Entities.Discord;
using Data.Entities.Tags;
using Disqord;
using Disqord.Bot.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Accessors;

/// <summary>
/// Allows retrieving current guild.
/// </summary>
public class DiscordGuildAccessor(
    ICommandContextAccessor commandContextAccessor,
    DataContext dbContext,
    IMemoryCache memoryCache,
    ILogger<DiscordGuildAccessor> logger)
    : CurrentContextAccessor<DiscordGuild, Snowflake>(commandContextAccessor, dbContext, memoryCache, logger)
{
    // Guilds are associated with their bot shard so we can allow this
    protected override TimeSpan CacheExpirationPeriod => TimeSpan.FromHours(1);

    public override bool CanBeAccessed(IDiscordCommandContext context)
        => context.GuildId.HasValue;

    public override Snowflake GetKey(IDiscordCommandContext context)
        => context.GuildId!.Value;

    protected override Task<DiscordGuild?> FetchAsync(Snowflake key, DataContext dbContext, CancellationToken ct)
        => dbContext.Guilds.FirstOrDefaultAsync(x => x.Id == key, ct);

    protected override DiscordGuild CreateDefault(Snowflake key) => new()
    {
        Id = key,
        Tags = new List<Tag>(),
        TagPrefix = DiscordGuild.DefaultTagPrefix,
        InlineTagsEnabled = false
    };
}