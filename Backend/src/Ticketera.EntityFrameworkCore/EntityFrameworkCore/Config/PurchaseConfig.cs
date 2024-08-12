using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.Entities;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class PurchaseConfig : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");
            builder.HasKey(x => x.Id);

            builder.Ignore(x => x.MercadoPagoPreference);
            builder.Ignore(x => x.MercadoPagoResponse);
        }
    }

}
