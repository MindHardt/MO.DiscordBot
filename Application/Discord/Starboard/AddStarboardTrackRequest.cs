using Application.Accessors;
using Data;
using Data.Entities.Starboard;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Starboard;

public record AddStarboardTrackRequest(
    Snowflake GuildId,
    string Emoji,
    int Threshold);

public class AddStarboardTrackHandler(
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext)
    : IRequestHandler<AddStarboardTrackRequest>
{
    public async Task HandleAsync(AddStarboardTrackRequest request, CancellationToken ct = default)
    {
        var emoji = LocalEmoji.FromString(request.Emoji).GetReactionFormat();

        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.None, true, ct);
        if (guild?.StarboardChannelId is null)
        {
            StarboardThrows.ThrowStarboardChannelNotAssigned();
        }

        var trackExists = await dataContext.StarboardTracks
            .AnyAsync(x =>
                x.GuildId == request.GuildId &&
                x.Emoji == emoji, ct);

        if (trackExists)
        {
            StarboardThrows.ThrowTrackExists(request.Emoji);
        }

        var newTrack = new StarboardTrack
        {
            GuildId = request.GuildId,
            Emoji = emoji,
            StarboardThreshold = request.Threshold
        };
        dataContext.StarboardTracks.Add(newTrack);
        await dataContext.SaveChangesAsync(ct);
    }
}