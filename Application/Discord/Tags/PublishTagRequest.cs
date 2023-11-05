﻿using Data;
using Disqord;

namespace Application.Discord.Tags;

public record PublishTagRequest(
    Snowflake GuildId,
    string TagName);

public class PublishTagHandler(
    TagService tagService,
    DataContext dataContext)
    : IRequestHandler<PublishTagRequest>
{
    public async Task HandleAsync(PublishTagRequest request, CancellationToken ct = default)
    {
        var tag = await tagService.FindExactAsync(request.GuildId, request.TagName, ct);
        if (tag is null)
        {
            TagThrows.ThrowTagNotFound(request.TagName);
        }

        tag.GuildId = null;
        await dataContext.SaveChangesAsync(ct);
    }
}