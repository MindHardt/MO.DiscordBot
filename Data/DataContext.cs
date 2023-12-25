using Data.Converters;
using Data.Entities.Discord;
using Data.Entities.Starboard;
using Data.Entities.Tags;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<DiscordUser> Users => Set<DiscordUser>();
    public DbSet<DiscordGuild> Guilds => Set<DiscordGuild>();
    public DbSet<StarboardTrack> StarboardTracks => Set<StarboardTrack>();
    public DbSet<StarboardQuote> StarboardQuotes => Set<StarboardQuote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Snowflake>()
            .HaveConversion<SnowflakeConverter>();

        base.ConfigureConventions(configurationBuilder);
    }
}