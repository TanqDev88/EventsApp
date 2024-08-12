using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketera.Enum;
using Ticketera.Events;
using Volo.Abp.Identity;

namespace Ticketera.Entities
{
    public class UserEvent : Base
    {
        public TypeUserEvent TypeUserEvent { get; set; } = TypeUserEvent.Readonly;
        public Guid IdentityUserId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
        public long EventId { get; set; }
        public virtual Event Event { get; set; }
    }
}
