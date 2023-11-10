using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Bot.Commands;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
public class BasicCommands : DiscordApplicationGuildModuleBase
{
    [SlashCommand("пинг"), Description("Проверка задержки бота")]
    public IResult Ping()
    {
        var startTime = new DateTimeOffset(Process.GetCurrentProcess().StartTime);

        var latency = DateTimeOffset.Now - Context.Interaction.CreatedAt();
        var embed = DiscordResponses.SuccessfulEmbed()
            .WithFields(
                new LocalEmbedField
                {
                    Name = Resources.Latency,
                    Value = Markdown.Code($"{latency.Milliseconds} мс")
                },
                new LocalEmbedField
                {
                    Name = Resources.StartedAt,
                    Value = startTime.ToDiscordTimestamp()
                });
        return Response(embed);
    }
}