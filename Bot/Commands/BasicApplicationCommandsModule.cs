using Disqord.Bot.Commands.Application;
using Qmmands;
using Services.Accessors;

namespace Bot.Commands;

public class BasicApplicationCommandsModule(DiscordUserAccessor userAccessor, DiscordGuildAccessor guildAccessor) : DiscordApplicationGuildModuleBase
{
    [SlashCommand("ping")]
    public async ValueTask<IResult> Ping()
    {
        var user = await userAccessor.GetAsync(Context, NotFoundEntityAction.Save);
        var guild = await guildAccessor.GetAsync(Context, NotFoundEntityAction.Save);
        return Response($"Pong! {user!.Id} - {guild!.Id}");
    }
}