using Data;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Starboard;

public record DeleteStarboardTrackRequest(
    Snowflake GuildId,
    string Emoji);

public class DeleteStarboardTrackHandler(
    DataContext dataContext)
    : IRequestHandler<DeleteStarboardTrackRequest>
{
    public async Task HandleAsync(DeleteStarboardTrackRequest request, CancellationToken ct = default)
    {
        var emoji = LocalEmoji.FromString(request.Emoji).GetReactionFormat();
        var existingTrack = await dataContext.StarboardTracks
            .FirstOrDefaultAsync(x =>
                x.GuildId == request.GuildId &&
                x.Emoji == emoji, ct);

        if (existingTrack is null)
        {
            StarboardThrows.ThrowTrackNotFound(request.Emoji);
        }

        dataContext.StarboardTracks.Remove(existingTrack);
        await dataContext.SaveChangesAsync(ct);
    }
}