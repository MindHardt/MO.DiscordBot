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

public class CreateTagHandler(
    TagFactory tagFactory,
    DiscordUserAccessor discordUserAccessor,
    DiscordGuildAccessor discordGuildAccessor,
    DataContext dataContext,
    TagNameService tagNameService)
    : IRequestHandler<CreateTagRequest>
{
    public async Task HandleAsync(CreateTagRequest request, CancellationToken ct)
    {
        if (tagNameService.ValidateTagName(request.TagName) is false)
        {
            throw new ArgumentException("Имя тега недопустимо!");
        }

        var user = await discordUserAccessor.GetAsync(request.AuthorId, NotFoundEntityAction.Create, false, ct);
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Create, false, ct);

        var tag = tagFactory.CreateMessageTag(request.TagName, request.Content, user, guild);
        dataContext.Tags.Add(tag);
        await dataContext.SaveChangesAsync(ct);
    }
}