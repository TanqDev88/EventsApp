using System;
using System.Collections.Generic;
using System.Text;
using Ticketera.Base;

namespace Ticketera.Events
{
    public class UserDto : BaseAuditedDto
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
