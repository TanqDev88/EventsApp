using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.TicketSectors;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class TicketSectorConfig : IEntityTypeConfiguration<TicketSector>
    {
        public void Configure(EntityTypeBuilder<TicketSector> builder)
        {
            builder.ToTable("TicketSectors");
            builder.HasKey(x => x.Id);

        }
    }
}
