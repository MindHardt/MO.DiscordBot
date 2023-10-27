using Application.Accessors;
using Data;
using Data.Entities.Tags;
using Disqord;
using MediatR;

namespace Application.Discord.Tags;

public record CreateTagRequest(
    Snowflake AuthorId,
    Snowflake GuildId,
    string TagName,
    string Content) : IRequest<MessageTag>;

public class CreateTagRequestHandler(
    TagFactory tagFactory,
    DiscordUserAccessor discordUserAccessor,
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext,
    TagNameService tagNameService)
    : IRequestHandler<CreateTagRequest, MessageTag>
{
    public async Task<MessageTag> HandleAsync(CreateTagRequest request, CancellationToken ct)
    {
        if (tagNameService.ValidateTagName(request.TagName) is false)
        {
            throw new ArgumentException("Имя тега недопустимо!");
        }

        var user = await discordUserAccessor.GetAsync(request.AuthorId, NotFoundEntityAction.Create, ct);
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Create, ct);

        var tag = tagFactory.CreateMessageTag(request.TagName, request.Content, user, guild);
        dataContext.Tags.Add(tag);
        await dataContext.SaveChangesAsync(ct);

        return tag;
    }
}