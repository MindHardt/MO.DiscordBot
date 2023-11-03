using Microsoft.EntityFrameworkCore;

namespace Data.Queries;

public static class CommonQueries
{
    /// <summary>
    /// If <paramref name="query"/> yields exactly one result, returns it.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="ct"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// The only <typeparamref name="T"/> in <paramref name="query"/> or
    /// <see langword="null"/> if there is none or several ones.</returns>
    public static async Task<T?> GetBestMatchAsync<T>(this IQueryable<T> query, CancellationToken ct = default)
        where T : class
    {
        try
        {
            return await query
                .Take(2)
                .SingleOrDefaultAsync(ct);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}