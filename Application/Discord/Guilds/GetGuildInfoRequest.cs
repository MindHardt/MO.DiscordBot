using Application.Accessors;
using Data;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Guilds;

public record GetGuildInfoRequest(
    Snowflake GuildId);

public record GuildInfo(
    Snowflake GuildId,
    bool InlineTagsEnabled,
    string InlineTagsPrefix,
    int TagsCount);

public class GetGuildInfoHandler(
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext)
    : IRequestHandler<GetGuildInfoRequest, GuildInfo>
{
    public async Task<GuildInfo> HandleAsync(GetGuildInfoRequest request, CancellationToken ct = default)
    {
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Save, false, ct);

        var guildInfo = await dataContext.Guilds
            .Where(x => x.Id == guild!.Id)
            .Select(x => new GuildInfo(x.Id, x.InlineTagsEnabled, x.TagPrefix, x.Tags.Count))
            .FirstAsync(ct);

        return guildInfo;
    }
}