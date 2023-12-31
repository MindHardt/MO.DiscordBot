﻿using Application.Accessors;
using Application.Discord;
using Application.Options;
using Bot;
using Data;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);

builder.UseSerilog((ctx, logger) => logger
   .ReadFrom.Configuration(ctx.Configuration)
   .Filter.ByExcluding(x => x.Exception is Disqord.WebSocket.WebSocketClosedException));

builder.ConfigureServices((ctx, services) =>
{
   var connStr = ctx.Configuration.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string DefaultConnection not found");

   services.AddDbContextFactory<DataContext>(options =>
      options.UseNpgsql(connStr));

   services.AddOptions<DiscordOptions>().BindConfiguration("Discord");
   services.AddOptions<FakerOptions>().BindConfiguration("Faker");

   services.AddAccessors();
   services.AddTags();
   services.AddHandlers();
});

builder.ConfigureDiscordBot<MoDiscordBot>((host, bot) =>
{
   bot.Token = host.Configuration["Discord:Token"]
      ?? throw new InvalidOperationException("Cannot retrieve discord bot token from configuration");
   bot.Intents = GatewayIntents.Unprivileged | GatewayIntents.MessageContent;
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
   await scope.ServiceProvider.GetRequiredService<DataContext>().Database.MigrateAsync();
}

await host.RunAsync();
//await Task.Delay(-1);