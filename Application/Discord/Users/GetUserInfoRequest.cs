using Application.Accessors;
using Data;
using Data.Entities.Discord;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Users;

public record GetUserInfoRequest(
    Snowflake UserId);

public record UserInfo(
    Snowflake UserId,
    DiscordUser.AccessLevel AccessLevel,
    int TagsCount);

public class GetUserInfoHandler(
    DiscordUserAccessor discordUserAccessor,
    DataContext dataContext)
    : IRequestHandler<GetUserInfoRequest, UserInfo>
{
    public async Task<UserInfo> HandleAsync(GetUserInfoRequest request, CancellationToken ct = default)
    {
        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Save, false, ct);

        var userInfo = await dataContext.Users
            .Where(x => x.Id == user!.Id)
            .Select(x => new UserInfo(x.Id, x.Access, x.Tags.Count))
            .FirstAsync(ct);

        return userInfo;
    }
}