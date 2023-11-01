using Application.Accessors;
using Data;
using Data.Entities.Tags;
using Disqord;

namespace Application.Discord.Tags;

public record RenameTagRequest(
    Snowflake GuildId,
    Snowflake UserId,
    string TagName,
    string NewTagName);

public class RenameTagHandler(
    GetTagHandler getTagHandler,
    DiscordUserAccessor discordUserAccessor,
    DataContext dataContext,
    TagNameService tagNameService)
    : IRequestHandler<RenameTagRequest>
{
    public async Task HandleAsync(RenameTagRequest request, CancellationToken ct = default)
    {
        tagNameService.ValidateTagName(request.NewTagName);

        var tagRequest = new GetTagRequest(request.GuildId, request.TagName);
        var tag = await getTagHandler.HandleAsync(tagRequest, ct);

        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Create, true, ct);
        if (tag.CanBeEditedBy(user!) is false)
        {
            throw new ArgumentException($"У вас нет права редактировать тег {tag.Name}");
        }

        tag.Name = request.NewTagName;
        await dataContext.SaveChangesAsync(ct);
    }
}