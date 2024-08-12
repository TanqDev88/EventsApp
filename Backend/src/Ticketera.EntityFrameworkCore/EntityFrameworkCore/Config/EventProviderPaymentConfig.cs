using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketera.Entities;

namespace Ticketera.EntityFrameworkCore.Config
{
    internal class EventProviderPaymentConfig : IEntityTypeConfiguration<EventProviderPayment>
    {
        public void Configure(EntityTypeBuilder<EventProviderPayment> builder)
        {
            builder.ToTable("EventProviderPayments");
            builder.HasKey(x => x.Id);

            builder.HasOne(p => p.Event)
                   .WithMany(b => b.EventProviders)
                   .HasForeignKey(p => p.EventId);

            builder.HasOne(p => p.ProviderPayment)
                       .WithMany(p => p.EventProviders)
                       .HasForeignKey(p => p.ProviderPaymentId);
        }
    }
}
