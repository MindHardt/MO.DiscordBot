using System.ComponentModel.DataAnnotations;
using Data.Entities.Discord;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Starboard;

public record StarboardQuote : IEntityTypeConfiguration<StarboardQuote>
{
    public required Snowflake OriginalMessageId { get; set; }
    public required Snowflake QuoteMessageId { get; set; }
    public required Snowflake DiscordGuildId { get; set; }
    public DiscordGuild DiscordGuild { get; set; } = null!;

    public void Configure(EntityTypeBuilder<StarboardQuote> builder)
    {
        builder.ToTable("StarboardQuotes");
        builder.HasKey(x => x.OriginalMessageId);

        builder.HasOne(x => x.DiscordGuild)
            .WithMany(x => x.StarboardQuotes)
            .HasForeignKey(x => x.DiscordGuildId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}