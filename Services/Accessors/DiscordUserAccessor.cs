using Data;
using Data.Entities.Discord;
using Data.Entities.Tags;
using Disqord.Bot.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Services.Accessors;

/// <summary>
/// Allows retrieving current user.
/// </summary>
public class DiscordUserAccessor(
    ICommandContextAccessor commandContextAccessor,
    ApplicationDbContext dbContext,
    IMemoryCache memoryCache,
    ILogger<DiscordUserAccessor> logger) 
    : CurrentContextAccessor<DiscordUser>(commandContextAccessor, dbContext, memoryCache, logger)
{
    public override bool CanBeAccessed(IDiscordCommandContext context)
        => true;

    protected override string GetContextKey(IDiscordCommandContext context)
        => context.AuthorId.ToString();

    protected override Task<DiscordUser?> FetchAsync(IDiscordCommandContext context, ApplicationDbContext dbContext, CancellationToken ct)
        => dbContext.Users.FirstOrDefaultAsync(x => x.Id == context.AuthorId, ct);

    protected override DiscordUser CreateDefault(IDiscordCommandContext context) => new()
    {
        Id = context.AuthorId,
        Access = DiscordUser.AccessLevel.Default,
        Tags = new List<Tag>()
    };
}