using Application.Accessors;
using Data.Entities.Discord;
using Disqord.Bot.Commands;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Bot.Attributes;

/// <summary>
/// Specifies that a command can only be executed by user with specified <see cref="DiscordUser.AccessLevel"/> or higher.
/// </summary>
/// <param name="accessLevel">The minimum required <see cref="DiscordUser.AccessLevel"/>.</param>
public class RequireAuthorAccessAttribute(DiscordUser.AccessLevel accessLevel) : DiscordCheckAttribute
{
    public override async ValueTask<IResult> CheckAsync(IDiscordCommandContext context)
    {
        var userAccessor = context.Services.GetRequiredService<DiscordUserAccessor>();
        var user = await userAccessor.GetAsync(NotFoundEntityAction.None, context, context.CancellationToken);

        return user?.Access >= accessLevel
            ? Results.Success
            : Results.Failure("Ваш уровень доступа не позволяет это");
    }
}