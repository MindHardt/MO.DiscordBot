using System.Diagnostics.CodeAnalysis;
using Bot.Attributes;
using Data.Entities.Discord;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Bot.Commands;

[SlashGroup("админ")]
[RequireAuthorAccess(DiscordUser.AccessLevel.Administrator)]
public class AdministratorCommands : DiscordApplicationGuildModuleBase
{
    public const string AdminUserOnly = "Только для администраторов бота";

    [SlashCommand("очистить-кеш"), Description(AdminUserOnly)]
    public IResult ClearCache()
    {
        var cache = Bot.Services.GetRequiredService<IMemoryCache>() as MemoryCache;
        cache?.Clear();

        return Response(DiscordResponses.SuccessfulEmbed());
    }
}