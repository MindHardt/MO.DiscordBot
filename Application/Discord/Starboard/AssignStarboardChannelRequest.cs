using Application.Accessors;
using Data;
using Disqord;

namespace Application.Discord.Starboard;

public record AssignStarboardChannelRequest(
    Snowflake GuildId,
    Snowflake ChannelId);

public class AssignStarboardChannelHandler(
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext)
    : IRequestHandler<AssignStarboardChannelRequest>
{
    public async Task HandleAsync(AssignStarboardChannelRequest request, CancellationToken ct = default)
    {
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Save, false, ct);

        guild!.StarboardChannelId = request.ChannelId;
        await dataContext.SaveChangesAsync(ct);
    }
}