using Application.Accessors;
using Data;
using Disqord;

namespace Application.Discord.Tags;

public record DeleteTagRequest(
    Snowflake GuildId,
    Snowflake UserId,
    string TagName);

public class DeleteTagHandler(
    DiscordUserAccessor discordUserAccessor,
    TagService tagService,
    DataContext dataContext)
    : IRequestHandler<DeleteTagRequest>
{
    public async Task HandleAsync(DeleteTagRequest request, CancellationToken ct = default)
    {
        var tag = await tagService.FindExactAsync(request.GuildId, request.TagName, ct);
        if (tag is null)
        {
            throw new ArgumentException($"Тег {request.TagName} не найден");
        }

        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Create, true, ct);
        if (tag.CanBeEditedBy(user!) is false)
        {
            throw new ArgumentException($"У вас нет прав на удаление тега {tag.Name}");
        }

        dataContext.Tags.Remove(tag);
        await dataContext.SaveChangesAsync(ct);
    }
}