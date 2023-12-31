﻿using Application;
using Application.Discord.Guilds;
using Bot.Attributes;
using Data.Entities.Discord;
using Disqord.Bot.Commands.Application;
using Disqord.Gateway;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace Bot.Commands;

[SlashGroup("сервер")]
public class GuildCommands : DiscordApplicationGuildModuleBase
{
    [SlashCommand("инфо"), Description("Информация о сервере")]
    public async ValueTask<IResult> Info()
    {
        await Deferral();

        var request = new GetGuildInfoRequest(Context.GuildId);
        var result = await Context.Services
            .GetRequiredService<GetGuildInfoHandler>()
            .HandleAsync(request, Context.CancellationToken)
            .AsResult();

        return result.Success
            ? Response(DiscordResponses.GuildInfoEmbed(result.Value, Bot.GetGuild(Context.GuildId)!))
            : Qmmands.Results.Failure(result.Exception.Message);
    }

    [SlashGroup("настроить")]
    public class ConfigureModule : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("префикс-тегов"), Description("Управляет префиксом строчных тегов")]
        public async ValueTask<IResult> TagPrefix(
            [Name("префикс"), Description("Новый префикс тегов")]
            string newPrefix)
        {
            await Deferral();

            var request = new SetTagPrefixRequest(Context.GuildId, newPrefix);
            var result = await Context.Services
                .GetRequiredService<SetTagPrefixHandler>()
                .HandleAsync(request, Context.CancellationToken)
                .AsResult();

            return result.Success
                ? Response(DiscordResponses.SuccessfulEmbed())
                : Qmmands.Results.Failure(result.Exception.Message);
        }

        [SlashCommand("строчные-теги"), Description("Управляет строчными тегами (которые пишутся в сообщении через префикс)")]
        [RequireAuthorAccess(DiscordUser.AccessLevel.Helper)]
        public async ValueTask<IResult> InlineTags(
            [Name("состояние"), Description("Включены ли строчные теги")]
            [Choice("✅ включены", "true"), Choice("❌ выключены", "false")]
            string enabled)
        {
            await Deferral();

            var tagsEnabled = bool.Parse(enabled);
            var request = new SetInlineTagsRequest(Context.GuildId, tagsEnabled);
            var result = await Context.Services
                .GetRequiredService<SetInlineTagsHandler>()
                .HandleAsync(request, Context.CancellationToken)
                .AsResult();

            return result.Success
                ? Response(DiscordResponses.SuccessfulEmbed($"Строчные теги на сервере - {result.Value.InlineTagsEnabled.ToEmoji()}"))
                : Qmmands.Results.Failure(result.Exception.Message);
        }
    }
}