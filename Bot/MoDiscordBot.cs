using Disqord;
using Disqord.Bot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot;

public class MoDiscordBot(
        IOptions<DiscordBotConfiguration> options,
        ILogger<MoDiscordBot> logger,
        IServiceProvider services,
        DiscordClient client)
    : DiscordBot(options, logger, services, client);