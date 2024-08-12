using Volo.Abp.Domain.Entities;
using Volo.Abp;

namespace Ticketera.Entities
{
    public abstract class Base : Entity<long>, ISoftDelete
    {
        public bool IsDeleted { get; set; }
    }
}
