using Application;
using Application.Discord.Tags;
using Bot.Attributes;
using Data.Entities.Discord;
using Data.Entities.Tags;
using Data.Projections;
using Disqord;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Results = Qmmands.Results;

namespace Bot.Commands;

[SlashGroup("тег")]
public class TagCommands : DiscordApplicationGuildModuleBase
{
    [SlashCommand("создать")]
    [Description("Создает тег и вашего текста")]
    [RequireAuthorAccess(DiscordUser.AccessLevel.Advanced)]
    public async ValueTask<IResult> CreateTag(
        [Maximum(Tag.MaxNameLength)]
        [Name("имя"), Description("имя нового тега")]
        string name,
        [Maximum(MessageTag.MaxContentLength)]
        [Name("содержимое"), Description("текст нового тега")]
        string content)
    {
        var request = new CreateTagRequest(Context.AuthorId, Context.GuildId, name, content);
        var result = await Context.Services
            .GetRequiredService<CreateTagRequestHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response("✅")
            : Results.Failure(result.Exception.Message);
    }

    [SlashCommand("отправить")]
    [Description("Отправляет тег с заданным именем")]
    public async ValueTask<IResult> SendTag(
        [Maximum(Tag.MaxNameLength)]
        [Name("имя"), Description("имя искомого тега")]
        string tagName)
    {
        var request = new GetTagRequest(Context.GuildId, tagName);
        var result = await Context.Services
            .GetRequiredService<GetTagRequestHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response(result.Value.Text)
            : Results.Failure(result.Exception.Message);
    }

    [SlashCommand("список")]
    [Description("Выводит список тегов, доступных на этом сервере")]
    public async ValueTask<IResult> ListTags(
        [Maximum(Tag.MaxNameLength)]
        [Name("запрос"), Description("часть имени тега")]
        string prompt = "")
    {
        var request = new ListTagsRequest(Context.GuildId, prompt, Discord.Limits.Message.Embed.MaxFieldAmount);
        var result = await Context.Services
            .GetRequiredService<ListTagsRequestHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        if (result.Success is false)
        {
            return Results.Failure(result.Exception.Message);
        }

        var embeds = result.Value
            .Select((tag, i) => new LocalEmbedField
            {
                Name = $"{i + 1}. {tag.Name}",
                Value = Markdown.Code(tag.TagKind switch
                {
                    TagKind.Message => "📧",
                    TagKind.Alias => "🔗",
                    _ => throw new ArgumentOutOfRangeException()
                }),
                IsInline = true
            });

        return Response(new LocalEmbed().WithTitle($"Теги по запросу {prompt}").WithFields(embeds));
    }
}