using Microsoft.Extensions.DependencyInjection;

namespace Application.Accessors;

public static class DependencyInjection
{
    /// <summary>
    /// Adds various <see cref="CurrentContextAccessor{TEntity, TKey}"/>s to <paramref name="services"/>.
    /// This includes <see cref="DiscordUserAccessor"/> and <see cref="DiscordGuildAccessor"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAccessors(this IServiceCollection services) => services
        .AddMemoryCache()
        .AddScoped<DiscordUserAccessor>()
        .AddScoped<DiscordGuildAccessor>();
}