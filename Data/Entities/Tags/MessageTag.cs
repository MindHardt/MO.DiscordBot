using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tags;

public record MessageTag : Tag, IEntity<MessageTag, MessageTagEntityConfiguration>
{
    [MaxLength(Disqord.Discord.Limits.Message.MaxContentLength)]
    public required string Content { get; set; }

    public override string Text => Content;
}

public class MessageTagEntityConfiguration : IEntityTypeConfiguration<MessageTag>
{
    public void Configure(EntityTypeBuilder<MessageTag> builder)
    {
    }
}