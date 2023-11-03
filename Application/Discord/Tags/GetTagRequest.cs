using Data.Entities.Tags;
using Disqord;
using MediatR;

namespace Application.Discord.Tags;

public record GetTagRequest(
    Snowflake GuildId,
    string Prompt)
    : IRequest<Tag>;

public class GetTagHandler(TagService tagService) : IRequestHandler<GetTagRequest, Tag>
{
    public async Task<Tag> HandleAsync(GetTagRequest request, CancellationToken ct)
    {
        return await tagService.FindSimilarAsync(request.GuildId, request.Prompt, ct)
            ?? throw new ArgumentException("Тег не найден");;
    }
}