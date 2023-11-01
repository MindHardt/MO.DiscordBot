using Application.Accessors;
using Data;
using Data.Entities.Tags;
using Disqord;

namespace Application.Discord.Tags;

public record CreateTagAliasRequest(
    Snowflake GuildId,
    Snowflake UserId,
    string OriginalTagName,
    string AliasName);

public class CreateTagAliasHandler(
    GetTagHandler getTagHandler,
    DiscordUserAccessor discordUserAccessor,
    DiscordGuildAccessor discordGuildAccessor,
    TagFactory tagFactory,
    TagNameService tagNameService,
    DataContext dataContext)
    : IRequestHandler<CreateTagAliasRequest>
{
    public async Task HandleAsync(CreateTagAliasRequest request, CancellationToken ct = default)
    {
        tagNameService.ValidateTagName(request.AliasName);

        var getTagRequest = new GetTagRequest(request.GuildId, request.OriginalTagName);
        var tag = await getTagHandler.HandleAsync(getTagRequest, ct);

        var messageTag = tag switch
        {
            MessageTag mt => mt,
            AliasTag at => at.ReferencedTag as MessageTag,
            _ => null
        };
        if (messageTag is null)
        {
            throw new ArgumentException("Ошибка получения тега");
        }

        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Create, false, ct);
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Create, false, ct);

        var alias = tagFactory.CreateAliasTag(request.AliasName, messageTag, user, guild);
        dataContext.Tags.Add(alias);
        await dataContext.SaveChangesAsync(ct);
    }
}