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
/// Allows retrieving current user.
/// </summary>
public class DiscordUserAccessor(
    ICommandContextAccessor commandContextAccessor,
    DataContext dbContext,
    IMemoryCache memoryCache,
    ILogger<DiscordUserAccessor> logger)
    : CurrentContextAccessor<DiscordUser, Snowflake>(commandContextAccessor, dbContext, memoryCache, logger)
{
    public override bool CanBeAccessed(IDiscordCommandContext context)
        => true;

    public override Snowflake GetKey(IDiscordCommandContext context)
        => context.AuthorId;

    protected override Task<DiscordUser?> FetchAsync(Snowflake key, DataContext dbContext, CancellationToken ct)
        => dbContext.Users.FirstOrDefaultAsync(x => x.Id == key, ct);

    protected override DiscordUser CreateDefault(Snowflake key) => new()
    {
        Id = key,
        Access = DiscordUser.AccessLevel.Default,
        Tags = new List<Tag>()
    };
}