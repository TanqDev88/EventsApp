using System;
using System.Collections.Generic;
using System.Text;
using Ticketera.Enum;

namespace Ticketera.Events
{
    public class UserEventDto
    {
        public string IdentityUserEmail { get; set; }
        public TypeUserEvent TypeUserEvent { get; set; } = TypeUserEvent.Readonly;
        public Guid IdentityUserId { get; set; }
    }
}