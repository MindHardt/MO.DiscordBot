using Data;
using Data.Projections;
using Data.Queries;
using Disqord;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Tags;

public record ListTagsRequest(
    Snowflake GuildId,
    string Prompt,
    int Limit)
    : IRequest<IReadOnlyCollection<TagOverview>>;

public class ListTagsRequestHandler(
    DataContext dataContext) 
    : IRequestHandler<ListTagsRequest, IReadOnlyCollection<TagOverview>>
{
    public async Task<IReadOnlyCollection<TagOverview>> HandleAsync(ListTagsRequest request, CancellationToken ct)
    {
        return await dataContext.Tags
            .VisibleIn(request.GuildId)
            .SearchByName(request.Prompt)
            .OrderBy(x => x.Name)
            .Take(request.Limit)
            .AsOverviews()
            .ToArrayAsync(ct);
    }
}