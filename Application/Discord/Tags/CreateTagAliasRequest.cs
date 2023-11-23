using Application.Accessors;
using Data;
using Data.Entities.Tags;
using Data.Queries;
using Disqord;
using Microsoft.EntityFrameworkCore;

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

        var tag = await dataContext.Tags
            .WhereVisibleIn(request.GuildId)
            .WhereNameExactly(request.OriginalTagName)
            .FirstOrDefaultAsync(ct);

        var messageTag = tag switch
        {
            MessageTag mt => mt,
            AliasTag at => at.ReferencedTag as MessageTag,
            _ => null
        };
        if (messageTag is null)
        {
            TagThrows.ThrowTagNotFound(request.OriginalTagName);
        }

        var tagExists = await dataContext.Tags
            .WhereVisibleIn(request.GuildId)
            .WhereNameExactly(request.AliasName)
            .AnyAsync(ct);
        if (tagExists)
        {
            TagThrows.ThrowTagNameOccupied(request.AliasName);
        }

        var user = await discordUserAccessor.GetAsync(request.UserId, NotFoundEntityAction.Create, false, ct);
        var guild = await discordGuildAccessor.GetAsync(request.GuildId, NotFoundEntityAction.Create, false, ct);

        var alias = tagFactory.CreateAliasTag(request.AliasName, messageTag, user, guild);
        dataContext.Tags.Add(alias);
        await dataContext.SaveChangesAsync(ct);
    }
}