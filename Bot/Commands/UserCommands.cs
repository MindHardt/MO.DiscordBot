using Application;
using Application.Discord.Users;
using Bot.Attributes;
using Data.Entities.Discord;
using Disqord;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Bot.Commands;

[SlashGroup("юзер")]
public class UserCommands : DiscordApplicationGuildModuleBase
{
    [SlashCommand("инфо"), Description("Получаем инфу о пользователе")]
    public async ValueTask<IResult> Info(
        [Name("пользователь"), Description("Тот чью инфу получаем")]
        IUser user)
    {
        var request = new GetUserInfoRequest(user.Id);
        var result = await Context.Services
            .GetRequiredService<GetUserInfoHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response(DiscordFormatter.CreateUserInfoEmbed(result.Value, user))
            : Qmmands.Results.Failure(result.Exception.Message);
    }

    [SlashCommand("уровень-доступа"), Description("Меняет уровень доступа юзера. Только для админов.")]
    [RequireAuthorAccess(DiscordUser.AccessLevel.Administrator)]
    public async ValueTask<IResult> SetAccessLevel(
        [Name("пользователь"), Description("Цель.")]
        IUser user,
        [Name("уровень"), Description("Уровень доступа который выставляем.")]
        DiscordUser.AccessLevel accessLevel,
        [Name("уведомить"), Description("Нужно ли упоминать пользователя")]
        [Choice("✅ да", "true"), Choice("❌ нет", "false")]
        string notification = "false")
    {
        var request = new SetUserAccessRequest(user.Id, accessLevel);
        var result = await Context.Services
            .GetRequiredService<SetUserAccessHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        if (result.Success is false)
        {
            return Qmmands.Results.Failure(result.Exception.Message);
        }

        var shouldNotify = bool.Parse(notification);
        var response = new LocalInteractionMessageResponse
            {
                Content = $"{user.Mention} теперь {accessLevel.ToName()}!",
                AllowedMentions = shouldNotify
                    ? LocalAllowedMentions.ExceptEveryone
                    : LocalAllowedMentions.None
            };
        return Response(response);
    }
}

public class MessageUserCommands : DiscordApplicationGuildModuleBase
{
    [UserCommand("Инфо")]
    public async ValueTask<IResult> Info(IUser user)
    {
        var request = new GetUserInfoRequest(user.Id);
        var result = await Context.Services
            .GetRequiredService<GetUserInfoHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response(DiscordFormatter.CreateUserInfoEmbed(result.Value, user))
            : Qmmands.Results.Failure(result.Exception.Message);
    }
}