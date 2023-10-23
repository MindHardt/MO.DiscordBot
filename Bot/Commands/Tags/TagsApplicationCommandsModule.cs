using Data;
using Data.Entities.Tags;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Services.Accessors;
using Services.Tags;

namespace Bot.Commands.Tags;

[SlashGroup("тег")]
public class TagsApplicationCommandsModule(IServiceProvider sp) : DiscordApplicationGuildModuleBase
{
    [SlashCommand("создать")]
    public async ValueTask<IResult> CreateTag(
        [Maximum(Tag.MaxNameLength)]
        [Name("имя"), Description("имя нового тега")]
        string name,
        [Maximum(MessageTag.MaxContentLength)]
        [Name("содержимое"), Description("текст нового тега")]
        string content)
    {
        var factory = sp.GetRequiredService<TagFactory>();
        var user = await sp.GetRequiredService<DiscordUserAccessor>().GetAsync(NotFoundEntityAction.Create);
        var guild = await sp.GetRequiredService<DiscordGuildAccessor>().GetAsync(NotFoundEntityAction.Create);
        var dbContext = sp.GetRequiredService<ApplicationDbContext>();

        var tag = factory.CreateMessageTag(name, content, user, guild);
        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        return Response("✅");
    }
}