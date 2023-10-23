using Data.Entities.Tags;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Tags;

public static class DependencyInjection
{
    /// <summary>
    /// Adds all infrastructure related to <see cref="Tag"/>s.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTags(this IServiceCollection services) => services
        .AddScoped<TagFactory>();
}