using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.Entities;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class FileAttachmentConfig : IEntityTypeConfiguration<FileAttachment>
    {
        public void Configure(EntityTypeBuilder<FileAttachment> builder)
        {
            builder.ToTable("FileAttachments");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Event)
                   .WithMany(x => x.FileAttachments)
                   .HasForeignKey(x => x.EventId)
                   .IsRequired(false);
        }
    }
}
