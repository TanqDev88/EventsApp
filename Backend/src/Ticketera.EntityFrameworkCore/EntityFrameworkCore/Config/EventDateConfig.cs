using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.Events;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class EventDateConfig : IEntityTypeConfiguration<EventDate>
    {
        public void Configure(EntityTypeBuilder<EventDate> builder)
        {
            builder.ToTable("EventDates");
            builder.HasKey(x => x.Id);

            builder.HasOne(p => p.Event)
                   .WithMany(b => b.EventDates)
                   .HasForeignKey(p => p.EventId);
        }
    }
}
