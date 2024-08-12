using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.ProviderPayments;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class ProviderPaymentConfig : IEntityTypeConfiguration<ProviderPayment>
    {
        public void Configure(EntityTypeBuilder<ProviderPayment> builder)
        {
            builder.ToTable("ProviderPayments");
            builder.HasKey(x => x.Id);

        }
    }
}
