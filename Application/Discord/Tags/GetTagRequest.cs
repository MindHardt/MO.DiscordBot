using Data;
using Data.Entities.Tags;
using Data.Queries;
using Disqord;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Tags;

public record GetTagRequest(
    Snowflake GuildId,
    string Prompt)
    : IRequest<Tag>;

public class GetTagRequestHandler(DataContext dataContext) : IRequestHandler<GetTagRequest, Tag>
{
    public async Task<Tag> HandleAsync(GetTagRequest request, CancellationToken ct)
    {
        var results = await dataContext.Tags
            .WithText()
            .VisibleIn(request.GuildId)
            .SearchByName(request.Prompt)
            .OrderBy(x => x.Name)
            .Take(2)
            .ToArrayAsync(ct);

        if (results.Length != 1)
        {
            throw new ArgumentException("Тег не найден");
        }

        return results[0];
    }
}