using System.Diagnostics.CodeAnalysis;
using Data;
using Disqord.Bot.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Accessors;

/// <summary>
/// A general abstraction above something being fetched from
/// the database according to <paramref name="commandContextAccessor"/>.
/// </summary>
/// <param name="commandContextAccessor"></param>
/// <param name="dbContext"></param>
public abstract class CurrentContextAccessor<TEntity, TKey>(
    ICommandContextAccessor commandContextAccessor,
    DataContext dbContext,
    IMemoryCache memoryCache,
    ILogger logger)
    where TEntity: class
    where TKey: notnull
{
    /// <summary>
    /// Gets current <typeparamref name="TEntity"/>.
    /// The value is cached for the lifetime of <see cref="CurrentContextAccessor{T, TKey}"/>
    /// so the consecutive calls will not lead to additional DB calls.
    /// </summary>
    /// <param name="notFoundAction">Specifies behaviour if entity is not found in the database.</param>
    /// <param name="context">
    /// The command context used for retrieval.
    /// If not specified, defaults to <see cref="ICommandContextAccessor.Context"/>.
    /// </param>
    /// <param name="allowCache">
    /// Defines whether method should check for cached values.
    /// It is advised to allow cache only for readonly operations.
    /// </param>
    /// <param name="ct"></param>
    /// <returns>
    /// The found or created <typeparamref name="TEntity"/> or
    /// <see langword="null"/> if user is not found and
    /// <paramref name="notFoundAction"/> doesn't allow creating one or if
    /// <see cref="TryGetKey"/> returns false.
    /// </returns>
    public Task<TEntity?> GetAsync(
        NotFoundEntityAction notFoundAction = NotFoundEntityAction.None,
        IDiscordCommandContext? context = null,
        bool allowCache = true,
        CancellationToken ct = default)
    {
        context ??= commandContextAccessor.Context;

        if (TryGetKey(context, out TKey? key))
        {
            return GetAsync(key, notFoundAction, allowCache, ct);
        }

        logger.LogDebug("Cannot access {Type} for command {Command}", typeof(TEntity).Name, context.Command?.Name);
        return Task.FromResult<TEntity?>(null);
    }

    /// <summary>
    /// Gets <see cref="TEntity"/> associated with <paramref name="key"/>.
    /// The value is cached for the lifetime of <see cref="CurrentContextAccessor{T, TKey}"/>
    /// so the consecutive calls will not lead to additional DB calls.
    /// </summary>
    /// <param name="key">The primary key of <typeparamref name="TEntity"/>.</param>
    /// <param name="notFoundAction">Specifies behaviour if entity is not found in the database.</param>
    /// <param name="allowCache">
    /// Defines whether method should check for cached values.
    /// It is advised to allow cache only for readonly operations.
    /// </param>
    /// <param name="ct"></param>
    /// <returns>
    /// The found or created <typeparamref name="TEntity"/> or
    /// <see langword="null"/> if user is not found and
    /// <paramref name="notFoundAction"/> doesn't allow creating one.
    /// </returns>
    public async Task<TEntity?> GetAsync(
        TKey key,
        NotFoundEntityAction notFoundAction = NotFoundEntityAction.None,
        bool allowCache = true,
        CancellationToken ct = default)
    {
        var cacheKey = GetCacheKey(key);
        if (allowCache && memoryCache.TryGetValue(cacheKey, out TEntity? entity))
        {
            logger.LogDebug("Entity with cache key {Key} retrieved from cache", cacheKey);
            return entity;
        }

        entity = await FetchAsync(key, dbContext, ct);
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

        entity = CreateDefault(key);
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
    /// Attempts to retrieve <typeparamref name="TKey"/> from <paramref name="context"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public abstract bool TryGetKey(IDiscordCommandContext context, [NotNullWhen(true)] out TKey? key);

    /// <summary>
    /// Defines how long <typeparamref name="TEntity"/> should be persisted in <see cref="IMemoryCache"/>.
    /// </summary>
    protected virtual TimeSpan CacheExpirationPeriod => TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets current entity key.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetUniqueCacheKey(TKey key)
        => key.ToString()!;

    /// <summary>
    /// Fetches <typeparamref name="TEntity"/> from the database.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dbContext"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected abstract Task<TEntity?> FetchAsync(TKey key, DataContext dbContext, CancellationToken ct);

    /// <summary>
    /// Creates default <typeparamref name="TEntity"/> for specified context.
    /// </summary>
    /// <returns></returns>
    protected abstract TEntity CreateDefault(TKey key);

    private string GetCacheKey(TKey key)
        => $"{typeof(TEntity).FullName}_{GetUniqueCacheKey(key)}";

    private void SetCache(string cacheKey, TEntity? entity)
        => memoryCache.Set(cacheKey, entity, CacheExpirationPeriod);
}

/// <summary>
/// Specifies behaviour of <see cref="CurrentContextAccessor{TEntity, TKey}"/> if entity is not found.
/// </summary>
public enum NotFoundEntityAction
{
    /// <summary>
    /// Entity will not be created, if none is found then <see langword="null"/> is returned.
    /// </summary>
    None,
    /// <summary>
    /// Entity is created and <see cref="DbSet{TEntity}.Add"/>ed to the <see cref="DataContext"/>,
    /// but <see cref="DbContext.SaveChangesAsync(CancellationToken)"/> is not called and value is not cached.
    /// </summary>
    Create,
    /// <summary>
    /// Entity is created and saved to cache and database.
    /// </summary>
    Save
}