using Application.Accessors;
using Application.Discord;
using Bot;
using Bot.Options;
using Data;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((ctx, services) =>
{
   var connStr = ctx.Configuration.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string DefaultConnection not found");

   services.AddDbContextFactory<DataContext>(options =>
      options.UseNpgsql(connStr));

   services.AddOptions<DiscordOptions>()
      .BindConfiguration("Discord");

   services.AddAccessors();
   services.AddTags();
   services.AddGuilds();
});

builder.ConfigureDiscordBot<MoDiscordBot>((host, bot) =>
{
   var discordOptions = host.Configuration.GetSection("Discord").Get<DiscordOptions>()
      ?? throw new InvalidOperationException($"Cannot bind {nameof(DiscordOptions)} from configuration");
   bot.Token = discordOptions.Token;
   bot.Intents = GatewayIntents.Unprivileged | GatewayIntents.MessageContent;
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
   await scope.ServiceProvider.GetRequiredService<DataContext>().Database.MigrateAsync();
}

await host.RunAsync();
//await Task.Delay(-1);