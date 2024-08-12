using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.TicketCategories;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class TicketCategoryConfig : IEntityTypeConfiguration<TicketCategory>
    {
        public void Configure(EntityTypeBuilder<TicketCategory> builder)
        {
            builder.ToTable("TicketCategories");
            builder.HasKey(x => x.Id);

        }
    }
}
