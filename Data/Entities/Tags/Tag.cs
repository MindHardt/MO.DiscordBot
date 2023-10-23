using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities.Discord;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tags;

/// <summary>
/// Represents an abstract tag, i.e. a saved message that can be pasted again by its name.
/// </summary>
public abstract record Tag : IEntity<Tag, TagEntityConfiguration>
{
    public const int MaxNameLength = 64;
    
    public int Id { get; set; }
    [MaxLength(MaxNameLength)]
    public required string Name { get; set; }

    [NotMapped]
    public abstract string Text { get; }

    public Snowflake? GuildId { get; set; }
    public DiscordGuild? Guild { get; set; }

    public Snowflake? OwnerId { get; set; }
    public DiscordUser? Owner { get; set; }
}

public class TagEntityConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.GuildId, x.Name });

        builder.HasDiscriminator()
            .HasValue<MessageTag>(nameof(MessageTag))
            .HasValue<AliasTag>(nameof(AliasTag));

        builder.HasOne(x => x.Guild)
            .WithMany(x => x.Tags)
            .HasForeignKey(x => x.GuildId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Owner)
            .WithMany(x => x.Tags)
            .HasForeignKey(x => x.OwnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}