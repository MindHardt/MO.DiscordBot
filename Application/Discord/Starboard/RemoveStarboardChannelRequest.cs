using Application.Accessors;
using Data;
using Disqord;

namespace Application.Discord.Starboard;

public record RemoveStarboardChannelRequest(
    Snowflake GuildId);

public class RemoveStarboardChannelHandler(
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext)
    : IRequestHandler<RemoveStarboardChannelRequest>
{
    public async Task HandleAsync(RemoveStarboardChannelRequest request, CancellationToken ct = default)
    {
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Save, false, ct);

        guild!.StarboardChannelId = null;
        await dataContext.SaveChangesAsync(ct);
    }
}