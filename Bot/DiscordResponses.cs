using Application.Discord.Guilds;
using Application.Discord.Users;
using Data.Entities.Discord;
using Disqord;
using Qommon;

namespace Bot;

public static class DiscordResponses
{
    public static LocalEmbed SuccessfulEmbed(string? additionalText = null) => new()
    {
        Title = Resources.Response_Successful,
        Timestamp = DateTimeOffset.Now,
        Description = Optional.FromNullable(additionalText),
        Color = Color.DarkSeaGreen
    };

    public static LocalEmbed GuildInfoEmbed(GuildInfo info, IGuild guild) =>
        new()
        {
            Title = guild.Name,
            ThumbnailUrl = Optional.FromNullable(guild.GetIconUrl()),
            Fields = new[]
            {
                new LocalEmbedField
                {
                    Name = Resources.Id,
                    Value = Markdown.CodeBlock(info.GuildId),
                    IsInline = false,
                },
                new LocalEmbedField
                {
                    Name = Resources.TagPrefix,
                    Value = Markdown.CodeBlock(info.InlineTagsPrefix),
                    IsInline = true
                },
                new LocalEmbedField
                {
                    Name = Resources.InlineTagsEnabled,
                    Value = Markdown.CodeBlock(info.InlineTagsEnabled.ToEmoji()),
                    IsInline = true
                },
                new LocalEmbedField
                {
                    Name = Resources.TagCount,
                    Value = Markdown.CodeBlock(info.TagsCount),
                    IsInline = true
                }
            }
        };

    public static LocalEmbed UserInfoEmbed(UserInfo info, IUser user) =>
        new()
        {
            Title = (user as IMember)?.Nick ?? user.Name,
            ImageUrl = user.GetAvatarUrl(CdnAssetFormat.Png, 2048),
            Fields = CreateDefaultEmbedFields(user)
                .Concat(CreateMemberEmbedFields(user))
                .Concat(CreateUserInfoEmbedFields(info))
                .ToArray()
        };

    public static IEnumerable<LocalEmbedField> CreateDefaultEmbedFields(IUser user)
    {
        return new LocalEmbedField[]
        {
            new()
            {
                Name = Resources.Url,
                Value = user.Mention,
                IsInline = false
            },
            new()
            {
                Name = Resources.Id,
                Value = Markdown.CodeBlock(user.Id),
                IsInline = false
            },
            new()
            {
                Name = Resources.Bot,
                Value = Markdown.CodeBlock(user.IsBot.ToEmoji()),
                IsInline = true
            }
        };
    }

    public static IEnumerable<LocalEmbedField> CreateMemberEmbedFields(IUser user)
    {
        return user is IMember member
            ? new LocalEmbedField[]
            {
                new()
                {
                    Name = Resources.Sound,
                    Value = Markdown.CodeBlock(member.IsDeafened ? "🔇" : "🔊"),
                    IsInline = true
                },
                new()
                {
                    Name = Resources.Microphone,
                    Value = Markdown.CodeBlock(member.IsMuted ? "🔇" : "🔊"),
                    IsInline = true
                }
            }
            : Array.Empty<LocalEmbedField>();
    }

    public static IEnumerable<LocalEmbedField> CreateUserInfoEmbedFields(UserInfo userInfo)
    {
        return new LocalEmbedField[]
        {
            new()
            {
                Name = Resources.TagCount,
                Value = Markdown.CodeBlock(userInfo.TagsCount),
                IsInline = false
            },
            new()
            {
                Name = Resources.AccessLevel,
                Value = Markdown.CodeBlock(userInfo.AccessLevel.ToName()),
                IsInline = false
            }
        };
    }
}