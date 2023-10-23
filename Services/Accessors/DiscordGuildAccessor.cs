using Data;
using Data.Entities.Discord;
using Data.Entities.Tags;
using Disqord.Bot.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Services.Accessors;

/// <summary>
/// Allows retrieving current guild.
/// </summary>
public class DiscordGuildAccessor(
    ICommandContextAccessor commandContextAccessor,
    ApplicationDbContext dbContext,
    IMemoryCache memoryCache,
    ILogger<DiscordGuildAccessor> logger)
    : CurrentContextAccessor<DiscordGuild>(commandContextAccessor, dbContext, memoryCache, logger)
{
    // Guilds are associated with their bot shard so we can allow this
    protected override TimeSpan CacheExpirationPeriod => TimeSpan.FromHours(1);

    public override bool CanBeAccessed(IDiscordCommandContext context)
        => context.GuildId.HasValue;

    protected override string GetContextKey(IDiscordCommandContext context)
        => context.GuildId.ToString()!;

    protected override Task<DiscordGuild?> FetchAsync(IDiscordCommandContext context, ApplicationDbContext dbContext, CancellationToken ct)
        => dbContext.Guilds.FirstOrDefaultAsync(x => x.Id == context.GuildId, ct);

    protected override DiscordGuild CreateDefault(IDiscordCommandContext context) => new()
    {
        Id = context.GuildId!.Value,
        Tags = new List<Tag>()
    };
}