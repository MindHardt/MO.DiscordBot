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
        var tag = await tagService.FindSimilarAsync(request.GuildId, request.Prompt, ct);

        if (tag is null)
        {
            TagThrows.ThrowTagNotFound(request.Prompt);
        }

        return tag;
    }
}