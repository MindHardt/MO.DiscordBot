using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Tags;

public record AliasTag : Tag, IEntityTypeConfiguration<AliasTag>
{
    public override string Text => ReferencedTag.Text;

    public int ReferencedTagId { get; set; }
    public required Tag ReferencedTag { get; set; }

    public void Configure(EntityTypeBuilder<AliasTag> builder)
    {
        builder.HasOne(x => x.ReferencedTag)
            .WithMany()
            .HasForeignKey(x => x.ReferencedTagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}