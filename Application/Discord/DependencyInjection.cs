using Application.Discord.Tags;
using Application.Options;
using Bogus;
using Data.Entities.Tags;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Application.Discord;

public static class DependencyInjection
{
    /// <summary>
    /// Adds all services and related to <see cref="Tag"/>s.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTags(this IServiceCollection services) => services
        .AddScoped<Faker>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FakerOptions>>().Value;
            return new Faker(options.Locale);
        })
        .AddScoped<TagFactory>()
        .AddScoped<TagNameService>();

    /// <summary>
    /// Adds all types that implement either
    /// <see cref="IRequestHandler{TRequest,TResult}"/>
    /// or <see cref="IRequestHandler{TRequest}"/>
    /// to <paramref name="services"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddHandlers(this IServiceCollection services) => services.Scan(scan =>
        scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(c => c.AssignableToAny(typeof(IRequestHandler<>), typeof(IRequestHandler<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
}