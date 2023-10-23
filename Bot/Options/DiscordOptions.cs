﻿using Disqord;

namespace Bot.Options;

public record DiscordOptions
{
    public required string Token { get; set; }
    public Snowflake[] OwnerIds { get; set; } = Array.Empty<Snowflake>();
}