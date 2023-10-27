using System.ComponentModel.DataAnnotations;
using Data.Entities.Tags;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Discord;

public record DiscordGuild : IEntity<DiscordGuild, DiscordGuildEntityConfiguration>, IDiscordEntity
{
    public const string DefaultTagPrefix = "$";
    public const int MaxTagPrefixLength = 16;

    public Snowflake Id { get; set; }

    /// <summary>
    /// A prefix used to find inline tags in messages.
    /// Only used if <see cref="InlineTagsEnabled"/>
    /// is set to <see langword="true"/>.
    /// </summary>
    [MaxLength(MaxTagPrefixLength)]
    public string TagPrefix { get; set; } = DefaultTagPrefix;

    /// <summary>
    /// Defines whether messages in this guild should be scanned for tag names.
    /// </summary>
    public bool InlineTagsEnabled { get; set; }

    public List<Tag> Tags { get; set; } = null!;
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

        builder.Property(x => x.TagPrefix)
            .HasDefaultValue(DiscordGuild.DefaultTagPrefix);
    }
}