using Microsoft.Extensions.DependencyInjection;

namespace Application.Accessors;

public static class DependencyInjection
{
    /// <summary>
    /// Adds various <see cref="CurrentContextAccessor{T}"/>s such as
    /// <see cref="DiscordUserAccessor"/> and <see cref="DiscordGuildAccessor"/> to <paramref name="services"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAccessors(this IServiceCollection services) => services
        .AddMemoryCache()
        .AddScoped<DiscordUserAccessor>()
        .AddScoped<DiscordGuildAccessor>();
}