using System;
using System.Collections.Generic;
using System.Text;
using Ticketera.Base;

namespace Ticketera.Tickets
{
    public class TicketOutDto 
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string TicketCategoryName { get; set; }
        public string TicketSectorName { get; set; }
        public DateTime EventDateStartDate { get; set; }
        public string EventPlace { get; set; }
        public bool SoldUsed { get; set; } = false;
    }
}
