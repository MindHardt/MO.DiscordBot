using Data;
using Data.Entities.Tags;
using Data.Queries;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Tags;

public record GetTagRequest(
    Snowflake GuildId,
    string Prompt);

public class GetTagHandler(
    DataContext dataContext)
    : IRequestHandler<GetTagRequest, Tag>
{
    public async Task<Tag> HandleAsync(GetTagRequest request, CancellationToken ct)
    {
        var tag = await dataContext.Tags
            .IncludeReferencedTag()
            .WhereVisibleIn(request.GuildId)
            .WhereNameExactly(request.Prompt)
            .FirstOrDefaultAsync(ct);

        if (tag is null)
        {
            TagThrows.ThrowTagNotFound(request.Prompt);
        }

        return tag;
    }
}