using Data;
using Data.Entities.Discord;
using Data.Entities.Starboard;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Starboard;

public record GetStarboardTracksRequest(
    Snowflake GuildId,
    int Limit);

public class GetStarboardTracksRequestHandler(
        DataContext dataContext)
    : IRequestHandler<GetStarboardTracksRequest, IReadOnlyCollection<StarboardTrack>>
{
    public async Task<IReadOnlyCollection<StarboardTrack>> HandleAsync(GetStarboardTracksRequest request, CancellationToken ct = default)
    {
        return await dataContext.StarboardTracks
            .Where(x => x.GuildId == request.GuildId)
            .OrderBy(x => x.Emoji)
            .Take(request.Limit)
            .ToArrayAsync(ct);
    }
}