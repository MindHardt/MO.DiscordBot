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
    DiscordUserAccessor discordUserAccessor,
    DiscordGuildAccessor discordGuildAccessor,
    TagFactory tagFactory,
    TagService tagService,
    DataContext dataContext)
    : IRequestHandler<CreateTagAliasRequest>
{
    public async Task HandleAsync(CreateTagAliasRequest request, CancellationToken ct = default)
    {
        tagService.ValidateTagName(request.AliasName);

        var tag = await tagService.FindSimilarAsync(request.GuildId, request.OriginalTagName, ct);

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

        var existingTag = await tagService.FindExactAsync(request.GuildId, request.AliasName, ct);
        if (existingTag is not null)
        {
            throw new ArgumentException($"Имя тега {request.AliasName} занято.");
        }

        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Create, false, ct);
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Create, false, ct);

        var alias = tagFactory.CreateAliasTag(request.AliasName, messageTag, user, guild);
        dataContext.Tags.Add(alias);
        await dataContext.SaveChangesAsync(ct);
    }
}