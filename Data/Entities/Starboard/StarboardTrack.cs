using System.ComponentModel.DataAnnotations;
using Data.Entities.Discord;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Starboard;

public record StarboardTrack :
    IEntityTypeConfiguration<StarboardTrack>
{
    public const int MaxEmojiLength = 64;

    public required Snowflake GuildId { get; set; }
    public DiscordGuild DiscordGuild { get; set; } = null!;

    [MaxLength(MaxEmojiLength)]
    public required string Emoji { get; set; }
    public int StarboardThreshold { get; set; }

    public void Configure(EntityTypeBuilder<StarboardTrack> builder)
    {
        builder.ToTable("StarboardTracks");
        builder.HasKey(x => new { x.GuildId, x.Emoji });

        builder.HasIndex(x => x.GuildId);

        builder.HasOne(x => x.DiscordGuild)
            .WithMany(x => x.StarboardTracks)
            .HasForeignKey(x => x.GuildId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}