using Data;
using Disqord.Bot.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Services.Accessors;

/// <summary>
/// A general abstraction above something being fetched from
/// the database according to <paramref name="commandContextAccessor"/>.
/// </summary>
/// <param name="commandContextAccessor"></param>
/// <param name="dbContext"></param>
public abstract class CurrentContextAccessor<T>(
    ICommandContextAccessor commandContextAccessor,
    ApplicationDbContext dbContext,
    IMemoryCache memoryCache,
    ILogger<CurrentContextAccessor<T>> logger)
    where T: class
{
    /// <summary>
    /// Gets current <typeparamref name="T"/>.
    /// The value is cached for the lifetime of <see cref="CurrentContextAccessor{TEntity}"/>
    /// so the consecutive calls will not lead to additional DB calls.
    /// </summary>
    /// <param name="context">
    /// The command context used for retrieval.
    /// If not specified, defaults to <see cref="ICommandContextAccessor.Context"/>.
    /// </param>
    /// <param name="notFoundAction">Specifies behaviour if entity is not found in the database.</param>
    /// <param name="ct"></param>
    /// <returns>
    /// The found or created <typeparamref name="T"/> or
    /// <see langword="null"/> if user is not found and
    /// <paramref name="notFoundAction"/> doesn't allow creating one or if
    /// <see cref="CanBeAccessed"/> returns false.
    /// </returns>
    public async Task<T?> GetAsync(
        IDiscordCommandContext? context = null,
        NotFoundEntityAction notFoundAction = NotFoundEntityAction.None,
        CancellationToken ct = default)
    {
        context ??= commandContextAccessor.Context;

        if (CanBeAccessed(context) is false)
        {
            logger.LogDebug("Cannot access {Type} for command {Command}", typeof(T).Name, context.Command?.Name);
            return null;
        }

        var cacheKey = GetCacheKey(context);
        if (memoryCache.TryGetValue(cacheKey, out T? entity))
        {
            logger.LogDebug("Entity with cache key {Key} retrieved from cache", cacheKey);
            return entity;
        }

        entity = await FetchAsync(context, dbContext, ct);
        SetCache(cacheKey, entity);
        if (entity is not null)
        {
            logger.LogDebug("Entity with cache key {Key} retrieved from database and cached", cacheKey);
            return entity;
        }

        if (notFoundAction is NotFoundEntityAction.None)
        {
            logger.LogDebug("Entity with cache key {Key} not found, returning null", cacheKey);
            return null;
        }

        entity = CreateDefault(context);
        logger.LogDebug("Entity with cache key {Key} created", cacheKey);
        dbContext.Add(entity);
        if (notFoundAction is not NotFoundEntityAction.Save)
        {
            return entity;
        }

        await dbContext.SaveChangesAsync(ct);
        SetCache(cacheKey, entity);
        logger.LogDebug("Entity with cache key {Key} saved to database", cacheKey);
        return entity;
    }

    /// <summary>
    /// Defines whether this <see cref="CurrentContextAccessor{T}"/> can retrieve its data.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract bool CanBeAccessed(IDiscordCommandContext context);

    /// <summary>
    /// Defines how long <typeparamref name="T"/> should be persisted in <see cref="IMemoryCache"/>.
    /// </summary>
    protected virtual TimeSpan CacheExpirationPeriod => TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets current entity key.
    /// </summary>
    /// <returns></returns>
    protected abstract string GetContextKey(IDiscordCommandContext context);

    /// <summary>
    /// Fetches <typeparamref name="T"/> from the database.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="dbContext"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected abstract Task<T?> FetchAsync(IDiscordCommandContext context, ApplicationDbContext dbContext, CancellationToken ct);

    /// <summary>
    /// Creates default <typeparamref name="T"/> for specified context.
    /// </summary>
    /// <returns></returns>
    protected abstract T CreateDefault(IDiscordCommandContext context);
    
    private string GetCacheKey(IDiscordCommandContext context)
        => $"{typeof(T).FullName}_{GetContextKey(context)}";

    private void SetCache(string cacheKey, T? entity)
        => memoryCache.Set(cacheKey, entity, CacheExpirationPeriod);
}

/// <summary>
/// Specifies behaviour of <see cref="CurrentContextAccessor{T}"/> if entity is not found.
/// </summary>
public enum NotFoundEntityAction
{
    /// <summary>
    /// User will not be created, if user is not found then <see langword="null"/> is returned.
    /// </summary>
    None,
    /// <summary>
    /// User is created and <see cref="DbSet{TEntity}.Add"/>ed to the <see cref="ApplicationDbContext"/>,
    /// but <see cref="DbContext.SaveChangesAsync(CancellationToken)"/> is not called and value is not cached.
    /// </summary>
    Create,
    /// <summary>
    /// User is created and saved to the database.
    /// </summary>
    Save
}