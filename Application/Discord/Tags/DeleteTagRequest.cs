using Application.Accessors;
using Data;
using Data.Queries;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Tags;

public record DeleteTagRequest(
    Snowflake GuildId,
    Snowflake UserId,
    string TagName);

public class DeleteTagHandler(
    DiscordUserAccessor discordUserAccessor,
    DataContext dataContext)
    : IRequestHandler<DeleteTagRequest>
{
    public async Task HandleAsync(DeleteTagRequest request, CancellationToken ct = default)
    {
        var tag = await dataContext.Tags
            .WhereVisibleIn(request.GuildId)
            .WhereNameExactly(request.TagName)
            .FirstOrDefaultAsync(ct);
        if (tag is null)
        {
            TagThrows.ThrowTagNotFound(request.TagName);
        }

        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Create, true, ct);
        if (tag.CanBeEditedBy(user!) is false)
        {
            TagThrows.ThrowAccessDenied();
        }

        dataContext.Tags.Remove(tag);
        await dataContext.SaveChangesAsync(ct);
    }
}