using Data.Entities.Tags;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Discord;

public record DiscordGuild : IEntity<DiscordGuild, DiscordGuildEntityConfiguration>, IDiscordEntity
{
    public Snowflake Id { get; set; }

    public List<Tag> Tags { get; set; } = new();
}

public class DiscordGuildEntityConfiguration : IEntityTypeConfiguration<DiscordGuild>
{
    public void Configure(EntityTypeBuilder<DiscordGuild> builder)
    {
        builder.HasMany(x => x.Tags)
            .WithOne(x => x.Guild)
            .HasForeignKey(x => x.GuildId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}