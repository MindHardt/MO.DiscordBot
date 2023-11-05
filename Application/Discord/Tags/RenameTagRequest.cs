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
            TagThrows.ThrowTagNotFound(request.TagName);
        }

        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Create, true, ct);
        if (tag.CanBeEditedBy(user!) is false)
        {
            TagThrows.ThrowAccessDenied();
        }

        var existingTag = await tagService.FindExactAsync(request.GuildId, request.NewTagName, ct);
        if (existingTag is null)
        {
            tag.Name = request.NewTagName;
            await dataContext.SaveChangesAsync(ct);
            return;
        }

        var canReplace = existingTag.CanBeEditedBy(user!);
        switch (canReplace, request.AllowReplace)
        {
            case { AllowReplace: false }:
                TagThrows.ThrowTagNameOccupied(request.NewTagName);
                break;
            case { canReplace: false, AllowReplace: true }:
                TagThrows.ThrowAccessDenied();
                break;
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