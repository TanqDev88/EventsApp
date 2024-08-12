using System;
using System.Collections.Generic;
using System.Text;

namespace Ticketera.Events
{
    public class TicketCountModelDto
    {
        public long TicketCategoryId { get; set; }
        public string TicketCategoryName { get; set; }
        public long TicketSectorId { get; set; }
        public string TicketSectorName { get; set; }
        public int Available { get; set; }
        public int Sold { get; set; }
        public int Total { get; set; }
        public decimal Price { get; set; }
        public String TimeLastPurchase { get; set; } = string.Empty;
        public decimal TotalSold { get; set; }
    }
}
