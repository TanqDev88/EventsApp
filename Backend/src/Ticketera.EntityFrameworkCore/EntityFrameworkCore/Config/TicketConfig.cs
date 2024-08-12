using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.Entities;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class TicketConfig : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");
            builder.HasKey(x => x.Id);

            builder.HasOne(p => p.Event)
                   .WithMany(b => b.Tickets)
                   .HasForeignKey(p => p.EventId);

            builder.HasOne(p => p.EventDate)
                   .WithMany(b => b.Tickets)
                   .HasForeignKey(p => p.EventDateId);

            builder.HasOne(p => p.TicketSector)
                   .WithMany(b => b.Tickets)
                   .HasForeignKey(p => p.TicketSectorId)
                   .IsRequired(false);

            builder.HasOne(p => p.TicketCategory)
                   .WithMany(b => b.Tickets)
                   .HasForeignKey(p => p.TicketCategoryId)
                   .IsRequired(false);


            builder.HasOne(p => p.Purchase)
                   .WithMany(b => b.Ticktes)
                   .HasForeignKey(p => p.PurchaseId);
        }
    }

}
