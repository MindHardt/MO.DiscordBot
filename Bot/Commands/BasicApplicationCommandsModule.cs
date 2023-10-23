using System.Diagnostics.CodeAnalysis;
using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Bot.Commands;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
public class BasicApplicationCommandsModule : DiscordApplicationGuildModuleBase
{
    [SlashCommand("ping")]
    public IResult Ping()
    {
        return Response($"Pong!");
    }

    [UserCommand("Инфо")]
    [SlashCommand("инфо")]
    public IResult Info(IUser user)
    {
        var embed = new LocalEmbed
        {
            ImageUrl = user.GetAvatarUrl(CdnAssetFormat.Png, 1024),
            Title = user.Name,
            Fields = CreateDefaultEmbedField(user).Concat(CreateMemberEmbedFields(user)).ToList()
        };
        return Response(embed);
    }

    #region Utility

    private IEnumerable<LocalEmbedField> CreateDefaultEmbedField(IUser user)
    {
        return new LocalEmbedField[]
        {
            new()
            {
                Name = "🔗 Ссылка",
                Value = user.Mention,
                IsInline = true
            },
            new()
            {
                Name = "🤖 Бот",
                Value = Markdown.Code(user.IsBot.ToEmoji()),
                IsInline = true
            },
            new()
            {
                Name = "🔑 ID",
                Value = Markdown.Code(user.Id),
                IsInline = true
            },
        };
    }

    private IEnumerable<LocalEmbedField> CreateMemberEmbedFields(IUser user)
    {
        return user is IMember member
            ? new LocalEmbedField[]
            {
                new()
                {
                    Name = "📛 Ник",
                    Value = Markdown.Code(member.Nick ?? "-"),
                    IsInline = true
                },
                new()
                {
                    Name = "🎧 Звук",
                    Value = Markdown.Code(member.IsDeafened ? "🔇" : "🔊"),
                    IsInline = true
                },
                new()
                {
                    Name = "🎙️ Микро",
                    Value = Markdown.Code(member.IsMuted ? "🔇" : "🔊"),
                    IsInline = true
                }
            }
            : Array.Empty<LocalEmbedField>();
    }

    #endregion
}