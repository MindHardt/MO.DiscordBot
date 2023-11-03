using Application.Accessors;
using Data;
using Data.Entities.Tags;
using Disqord;

namespace Application.Discord.Tags;

public record RenameTagRequest(
    Snowflake GuildId,
    Snowflake UserId,
    string TagName,
    string NewTagName,
    bool AllowReplace);

public class RenameTagHandler(
    DiscordUserAccessor discordUserAccessor,
    DataContext dataContext,
    TagService tagService)
    : IRequestHandler<RenameTagRequest>
{
    public async Task HandleAsync(RenameTagRequest request, CancellationToken ct = default)
    {
        tagService.ValidateTagName(request.NewTagName);

        var tag = await tagService.FindSimilarAsync(request.GuildId, request.TagName, ct);
        if (tag is null)
        {
            throw new ArgumentException($"Тег {request.TagName} не найден");
        }

        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Create, true, ct);
        if (tag.CanBeEditedBy(user!) is false)
        {
            throw new ArgumentException($"У вас нет права редактировать тег {tag.Name}");
        }

        var existingTag = await tagService.FindExactAsync(request.GuildId, request.NewTagName, ct);
        if (existingTag is null)
        {
            tag.Name = request.NewTagName;
            await dataContext.SaveChangesAsync(ct);
            return;
        }

        var canReplace = existingTag.CanBeEditedBy(user!);
        var exceptionMessage = (canReplace, request.AllowReplace) switch
        {
            { canReplace: true, AllowReplace: true } => null,

            { canReplace: true, AllowReplace: false } =>
                $"Тег {request.NewTagName} существует. Если вы хотите заменить его укажите это при выполнении команды.",

            { canReplace: false, AllowReplace: true } =>
                $"Тег {request.NewTagName} существует и у вас нет права на его перезапись.",

            { canReplace: false, AllowReplace: false } =>
                $"Имя тега {request.NewTagName} уже занято."
        };
        if (exceptionMessage is not null)
        {
            throw new InvalidOperationException(exceptionMessage);
        }

        switch (existingTag)
        {
            case MessageTag messageTag:
                messageTag.Content = tag.Text;
                dataContext.Tags.Remove(tag);
                await dataContext.SaveChangesAsync(ct);
                break;
            case AliasTag:
                tag.Name = request.NewTagName;
                dataContext.Tags.Remove(existingTag);
                await dataContext.SaveChangesAsync(ct);
                break;
        }
    }
}