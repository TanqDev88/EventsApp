using System;
using System.Collections.Generic;
using Ticketera.Entities;

namespace Ticketera.TicketSectors
{
    public class TicketSector : Base
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatorId { get; set; }

        public virtual IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
