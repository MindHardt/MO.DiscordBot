using Application.Accessors;
using Data;
using Data.Entities.Discord;
using Disqord;

namespace Application.Discord.Users;

public record SetUserAccessRequest(
    Snowflake UserId,
    DiscordUser.AccessLevel AccessLevel);

public class SetUserAccessHandler(
    DiscordUserAccessor discordUserAccessor,
    DataContext dataContext)
    : IRequestHandler<SetUserAccessRequest>
{
    public async Task HandleAsync(SetUserAccessRequest request, CancellationToken ct = default)
    {
        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Save, false, ct);

        user!.Access = request.AccessLevel;
        dataContext.Users.Update(user);
        await dataContext.SaveChangesAsync(ct);
    }
}