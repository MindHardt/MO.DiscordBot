using Application.Accessors;
using Data;
using Data.Entities.Discord;
using Disqord;

namespace Application.Discord.Guilds;

public record SetInlineTagsRequest(
    Snowflake GuildId,
    bool InlineTagsEnabled);

public class SetInlineTagsHandler(
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext)
    : IRequestHandler<SetInlineTagsRequest, DiscordGuild>
{
    public async Task<DiscordGuild> HandleAsync(SetInlineTagsRequest request, CancellationToken ct = default)
    {
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Save, false, ct);
        guild!.InlineTagsEnabled = request.InlineTagsEnabled;
        await dataContext.SaveChangesAsync(ct);

        return guild;
    }
}