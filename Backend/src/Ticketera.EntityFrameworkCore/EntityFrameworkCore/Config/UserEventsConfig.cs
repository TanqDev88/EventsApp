using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.Entities;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class UserEventsConfig : IEntityTypeConfiguration<UserEvent>
    {
        public void Configure(EntityTypeBuilder<UserEvent> builder)
        {
            builder.ToTable("UserEvents");
            builder.HasKey(x => x.Id);

            builder.HasOne(p => p.Event)
                   .WithMany(b => b.UserEvents)
                   .HasForeignKey(p => p.EventId);

            builder.HasOne(p => p.IdentityUser)
                   .WithMany()
                   .HasForeignKey(p => p.IdentityUserId);
        }
    }
}
