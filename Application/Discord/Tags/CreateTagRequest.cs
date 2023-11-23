using Application.Accessors;
using Data;
using Data.Queries;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Application.Discord.Tags;

public record CreateTagRequest(
    Snowflake AuthorId,
    Snowflake GuildId,
    string TagName,
    string Content);

public class CreateTagHandler(
    TagFactory tagFactory,
    DiscordUserAccessor discordUserAccessor,
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext,
    TagService tagService)
    : IRequestHandler<CreateTagRequest>
{
    public async Task HandleAsync(CreateTagRequest request, CancellationToken ct)
    {
        tagService.ValidateTagName(request.TagName);

        var user = await discordUserAccessor.GetAsync(request.AuthorId, NotFoundEntityAction.Create, false, ct);
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Create, false, ct);

        var nameOccupied = await dataContext.Tags
            .WhereVisibleIn(request.GuildId)
            .WhereNameExactly(request.TagName)
            .AnyAsync(ct);
        if (nameOccupied)
        {
            TagThrows.ThrowTagNameOccupied(request.TagName);
        }

        var tag = tagFactory.CreateMessageTag(request.TagName, request.Content, user, guild);
        dataContext.Tags.Add(tag);
        await dataContext.SaveChangesAsync(ct);
    }
}