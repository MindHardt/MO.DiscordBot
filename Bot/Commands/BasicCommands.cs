using System.Diagnostics.CodeAnalysis;
using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Bot.Commands;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
public class BasicCommands : DiscordApplicationGuildModuleBase
{
    [SlashCommand("пинг")]
    [Description("Проверка задержки бота")]
    public IResult Ping()
    {
        var latency = DateTimeOffset.Now - Context.Interaction.CreatedAt();
        var embed = new LocalEmbed()
            .WithTitle("Pong!")
            .WithFields(new LocalEmbedField
            {
                Name = "⏱️ задержка",
                Value = Markdown.Code($"{latency.Milliseconds} мс")
            });
        return Response(embed);
    }
}