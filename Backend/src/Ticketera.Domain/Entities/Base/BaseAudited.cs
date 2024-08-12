using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;

namespace Ticketera.Entities
{
    public abstract class BaseAudited : FullAuditedEntity<long>, ISoftDelete
    {
        public BaseAudited()
        {
            this.CreationTime = DateTime.UtcNow;
        }
    }
}
