using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketera.Events
{
    public class UserOutDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
