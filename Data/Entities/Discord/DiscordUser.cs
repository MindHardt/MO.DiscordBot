using System.ComponentModel.DataAnnotations;
using Data.Entities.Tags;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Discord;

public record DiscordUser : IEntity<DiscordUser, DiscordUserEntityConfiguration>, IDiscordEntity
{
    public Snowflake Id { get; set; }

    public AccessLevel Access { get; set; }

    public List<Tag> Tags { get; set; } = null!;

    public enum AccessLevel : byte
    {
        [Display(Name = "Стандартный")]
        Default = 0,

        [Display(Name = "Продвинутый")]
        Advanced = 1,

        [Display(Name = "Помощник")]
        Helper = 2,

        [Display(Name = "Администратор")]
        Administrator = 3
    }
}

public class DiscordUserEntityConfiguration : IEntityTypeConfiguration<DiscordUser>
{
    public void Configure(EntityTypeBuilder<DiscordUser> builder)
    {
        builder.HasMany(x => x.Tags)
            .WithOne(x => x.Owner)
            .HasForeignKey(x => x.OwnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}