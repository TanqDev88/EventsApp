using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ticketera.Events;

namespace Ticketera.EntityFrameworkCore.Config
{
    public class EventConfig : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");
            builder.HasKey(x => x.Id);

            builder.Ignore(x => x.Prices);
            builder.Ignore(x => x.SaleForPerson);
            builder.Ignore(x => x.IsActive);
            builder.Ignore(x => x.BackgroundJobId);
        }
    }
   
}
