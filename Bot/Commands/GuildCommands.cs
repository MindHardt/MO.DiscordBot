using Application;
using Application.Discord.Guilds;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Results = Qmmands.Results;

namespace Bot.Commands;

[SlashGroup("сервер")]
public class GuildCommands : DiscordApplicationGuildModuleBase
{
    [SlashGroup("настроить")]
    public class ConfigureModule : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("префикс-тегов")]
        public async ValueTask<IResult> TagPrefix(
            [Name("префикс")]
            [Description("Новый префикс тегов")]
            string newPrefix)
        {
            var request = new SetTagPrefixRequest(Context.GuildId, newPrefix);
            var result = await Context.Services
                .GetRequiredService<SetTagPrefixHandler>()
                .HandleAsync(request, Context.CancellationToken)
                .AsResult();

            return result.Success
                ? Response("✅")
                : Results.Failure(result.Exception.Message);
        }
    }
}