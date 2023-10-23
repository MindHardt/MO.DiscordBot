using Bot;
using Bot.Options;
using Data;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Accessors;
using Services.Tags;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((ctx, services) =>
{
   var connStr = ctx.Configuration.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string DefaultConnection not found");

   services.AddDbContextFactory<ApplicationDbContext>(options =>
      options.UseNpgsql(connStr));

   services.AddOptions<DiscordOptions>()
      .BindConfiguration("Discord");

   services.AddAccessors();
   services.AddTags();
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
   await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
}

await host.RunAsync();
//await Task.Delay(-1);