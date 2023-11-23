using Data;
using Data.Projections;
using Data.Queries;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Tags;

public record ListTagsRequest(
    Snowflake GuildId,
    string Prompt,
    int Limit);

public class ListTagsHandler(
    DataContext dataContext)
    : IRequestHandler<ListTagsRequest, IReadOnlyCollection<TagOverview>>
{
    public async Task<IReadOnlyCollection<TagOverview>> HandleAsync(ListTagsRequest request, CancellationToken ct)
    {
        return await dataContext.Tags
            .WhereVisibleIn(request.GuildId)
            .WhereNameLike(request.Prompt)
            .OrderBy(x => x.Name)
            .Take(request.Limit)
            .AsOverviews()
            .ToArrayAsync(ct);
    }
}