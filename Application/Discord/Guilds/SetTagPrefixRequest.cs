using Application.Accessors;
using Application.Discord.Tags;
using Data;
using Disqord;

namespace Application.Discord.Guilds;

public record SetTagPrefixRequest(
    Snowflake GuildId,
    string NewPrefix);

public class SetTagPrefixHandler(
    TagNameService tagNameService,
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext)
    : IRequestHandler<SetTagPrefixRequest>
{
    public async Task HandleAsync(SetTagPrefixRequest request, CancellationToken ct = default)
    {
        tagNameService.ValidateGuildPrefix(request.NewPrefix);

        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Save, false, ct);
        guild!.TagPrefix = request.NewPrefix;

        dataContext.Guilds.Update(guild);
        await dataContext.SaveChangesAsync(ct);
    }
}