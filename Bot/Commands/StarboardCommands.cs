using Application;
using Application.Discord.Starboard;
using Bot.Attributes;
using Data.Entities.Discord;
using Disqord;
using Disqord.Bot.Commands;
using Disqord.Bot.Commands.Application;
using Disqord.Models;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Bot.Commands;

[SlashGroup("апвоут")]
[RequireAuthorPermissions(Permissions.Administrator)]
[RequireAuthorAccess(DiscordUser.AccessLevel.Helper)]
public class StarboardCommands : DiscordApplicationGuildModuleBase
{
    [SlashCommand("сюда"), Description("Отмечает этот канал как доску почета куда будут приходить апвоуты")]
    public async ValueTask<IResult> SetStarboardChannel()
    {
        await Deferral();

        var request = new AssignStarboardChannelRequest(Context.GuildId, Context.ChannelId);
        var result = await Context.Services
            .GetRequiredService<AssignStarboardChannelHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response(DiscordResponses.SuccessfulEmbed($"Назначил канал {Context.GetChannelLink()} доской почета"))
            : Qmmands.Results.Failure(result.Exception.Message);
    }

    [SlashCommand("отслеживать"), Description("Начинает отслеживать данную реакцию для доски почета")]
    public async ValueTask<IResult> AddTrack(
        [Name("реакция"), Description("Отслеживаемая реакция")]
        string emoji,
        [Name("количество"), Description("Сколько нужно реакций для попадания на доску почета")]
        int threshold = 3)
    {
        await Deferral();

        var request = new AddStarboardTrackRequest(Context.GuildId, emoji, threshold);
        var result = await Context.Services
            .GetRequiredService<AddStarboardTrackHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response(DiscordResponses.SuccessfulEmbed($"Отслеживаю реакцию {emoji}"))
            : Qmmands.Results.Failure(result.Exception.Message);
    }

    [SlashCommand("не-отслеживать"), Description("Перестает отслеживать данную реакцию для доски почета")]
    public async ValueTask<IResult> DeleteTrack(
        string emoji)
    {
        await Deferral();

        var request = new DeleteStarboardTrackRequest(Context.GuildId, emoji);
        var result = await Context.Services
            .GetRequiredService<DeleteStarboardTrackHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response(DiscordResponses.SuccessfulEmbed($"Больше не отслеживаю реакцию {emoji}"))
            : Qmmands.Results.Failure(result.Exception.Message);
    }
}