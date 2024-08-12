using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketera.Events
{
    public class TicketStatisticsDto
    {
        public List<TicketCountModelDto> TicketCounts { get; set; }
        public string LastTimePurchase { get; set; } = string.Empty;
    }
}
