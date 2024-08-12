using System;

namespace Ticketera.Tickets
{
    public class PurchaseTicketInputDto
    {
        public int Count { get; set; } = 0;
        public string Code { get; set; }
        public long EventId { get; set; }
        public long EventDateId { get; set; }
        public long? TicketCategoryId { get; set; }
        public long? TicketSectorId { get; set; }
    }
}
