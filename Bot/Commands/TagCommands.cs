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
            .GetRequiredService<CreateTagHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response("✅")
            : Qmmands.Results.Failure(result.Exception.Message);
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
            .GetRequiredService<GetTagHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response(result.Value.Text)
            : Qmmands.Results.Failure(result.Exception.Message);
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
            .GetRequiredService<ListTagsHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        if (result.Success is false)
        {
            return Qmmands.Results.Failure(result.Exception.Message);
        }

        var tagLines = result.Value
            .Select((tag, i) => (
                Number: i + 1,
                tag.Name,
                TagKind: tag.TagKind switch
                {
                    TagKind.Message => "📧",
                    TagKind.Alias => "🔗",
                    _ => throw new ArgumentOutOfRangeException()
                }))
            .Select(x => $"{x.TagKind}| {x.Number}. {x.Name}");

        var embed = new LocalEmbed
        {
            Title = $"Теги по запросу [{prompt}]",
            Description = Markdown.CodeBlock(string.Join('\n', tagLines)),
        };
        return Response(embed);
    }

    [SlashCommand("переименовать")]
    [Description("Переименовывает тег")]
    public async ValueTask<IResult> RenameTag(
        [Maximum(Tag.MaxNameLength)] [Name("имя"), Description("Имя искомого тега")]
        string tagName,
        [Maximum(Tag.MaxNameLength)] [Name("новое-имя"), Description("Новое имя тега")]
        string newName)
    {
        var request = new RenameTagRequest(Context.GuildId, Context.AuthorId, tagName, newName);
        var result = await Context.Services
            .GetRequiredService<RenameTagHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response($"Переименовал тег {Markdown.Code(newName)}")
            : Qmmands.Results.Failure(result.Exception.Message);
    }

    [SlashCommand("удалить")]
    [Description("Удаляет тег, к которому у вас есть доступ")]
    public async ValueTask<IResult> DeleteTag(
        [Name("имя"), Description("Имя удаляемого тега")]
        string tagName)
    {
        var request = new DeleteTagRequest(Context.GuildId, Context.AuthorId, tagName);
        var result = await Context.Services
            .GetRequiredService<DeleteTagHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response("Успешно удалил тег ✅")
            : Qmmands.Results.Failure(result.Exception.Message);
    }

    [SlashCommand("синоним")]
    [Description("Создает синоним для тега, позволяя вызывать его по новому имени.")]
    public async ValueTask<IResult> CreateTagAlias(
        [Name("имя"), Description("Имя удаляемого тега")]
        string tagName,
        [Maximum(Tag.MaxNameLength)]
        [Name("синоним"), Description("Имя синонима тега")]
        string aliasName)
    {
        var request = new CreateTagAliasRequest(Context.GuildId, Context.AuthorId, tagName, aliasName);
        var result = await Context.Services
            .GetRequiredService<CreateTagAliasHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response($"Создал синоним {Markdown.Code(aliasName)}")
            : Qmmands.Results.Failure(result.Exception.Message);
    }

    [AutoComplete("переименовать")]
    [AutoComplete("отправить")]
    [AutoComplete("удалить")]
    [AutoComplete("синоним")]
    public async ValueTask TagNameAutocomplete(
        [Name("имя")] AutoComplete<string> tagName)
    {
        if (tagName.IsFocused is false)
        {
            return;
        }

        const int limit = Discord.Limits.ApplicationCommand.MaxOptionAmount;
        var request = new ListTagsRequest(Context.GuildId, tagName.RawArgument!, limit);
        var result = await Context.Services
            .GetRequiredService<ListTagsHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        if (result.Success)
        {
            tagName.Choices.AddRange(result.Value.Select(x => x.Name));
        }
    }
}

public class MessageTagCommands : DiscordApplicationGuildModuleBase
{
    [MessageCommand("Создать тег")]
    public async ValueTask<IResult> CreateTag(IMessage message)
    {
        var name = Context.Services
            .GetRequiredService<TagNameService>()
            .GenerateRandomTagName();
        var attachmentLines = (message as IUserMessage)?.Attachments
            .Select(x => $"\n{x.Url}") ?? Array.Empty<string>();
        var content = message.Content + string.Concat(attachmentLines);

        var request = new CreateTagRequest(Context.AuthorId, Context.GuildId, name, content);
        var result = await Context.Services
            .GetRequiredService<CreateTagHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response($"Создал тег {Markdown.Code(name)}")
            : Qmmands.Results.Failure(result.Exception.Message);
    }
}