using Data;
using Data.Queries;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Tags;

public record PublishTagRequest(
    Snowflake GuildId,
    string TagName);

public class PublishTagHandler(
    DataContext dataContext)
    : IRequestHandler<PublishTagRequest>
{
    public async Task HandleAsync(PublishTagRequest request, CancellationToken ct = default)
    {
        var tag = await dataContext.Tags
            .WhereVisibleIn(request.GuildId)
            .WhereNameExactly(request.TagName)
            .FirstOrDefaultAsync(ct);
        if (tag is null)
        {
            TagThrows.ThrowTagNotFound(request.TagName);
        }

        tag.GuildId = null;
        await dataContext.SaveChangesAsync(ct);
    }
}