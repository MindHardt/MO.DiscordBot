using Data;
using Data.Entities.Tags;
using Disqord;

namespace Application.Discord.Tags;

public record RenameTagRequest(
    Snowflake GuildId,
    string TagName,
    string NewTagName);

public class RenameTagHandler(
    GetTagHandler getTagHandler,
    DataContext dataContext,
    TagNameService tagNameService)
    : IRequestHandler<RenameTagRequest>
{
    public async Task HandleAsync(RenameTagRequest request, CancellationToken ct = default)
    {
        if (tagNameService.ValidateTagName(request.NewTagName) is false)
        {
            throw new ArgumentException($"Имя тега {request.NewTagName} недопустимо");
        }

        var tagRequest = new GetTagRequest(request.GuildId, request.TagName);
        var tag = await getTagHandler.HandleAsync(tagRequest, ct);

        tag.Name = request.NewTagName;
        await dataContext.SaveChangesAsync(ct);
    }
}