using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tags;

public record AliasTag : Tag, IEntity<AliasTag, AliasTagEntityConfiguration>
{
    public override string Text => ReferencedTag.Text;

    public int ReferencedTagId { get; set; }
    public required Tag ReferencedTag { get; set; }
}

public class AliasTagEntityConfiguration : IEntityTypeConfiguration<AliasTag>
{
    public void Configure(EntityTypeBuilder<AliasTag> builder)
    {
        builder.HasOne(x => x.ReferencedTag)
            .WithMany()
            .HasForeignKey(x => x.ReferencedTagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}