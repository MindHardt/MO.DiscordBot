using Application.Discord.Guilds;
using Application.Discord.Tags;
using Data.Entities.Discord;
using Data.Entities.Tags;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Discord;

public static class DependencyInjection
{
    /// <summary>
    /// Adds all services and handlers related to <see cref="Tag"/>s.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTags(this IServiceCollection services) => services
        .AddScoped<CreateTagRequestHandler>()
        .AddScoped<GetTagRequestHandler>()
        .AddScoped<ListTagsRequestHandler>()
        .AddScoped<TagFactory>()
        .AddScoped<TagNameService>();

    /// <summary>
    /// Adds all services and handlers related to <see cref="DiscordGuild"/>s.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddGuilds(this IServiceCollection services) => services
        .AddScoped<SetTagPrefixHandler>();
}