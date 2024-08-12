using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketera.Events
{
    public class DashboardStatisticsDto
    {
        public int TotalActiveEvents { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TicketsSoldToday { get; set; }
    }

}
