using Ticketera.Base;

namespace Ticketera.Tickets
{
    public class PurchaseTicketDto
    {
        public int Count { get; set; } = 0;
        public long? TicketCategoryId { get; set; }
        public long? TicketSectorId { get; set; }
        public decimal Price { get; set; } = 0;
        public string TicketCategoryName { get; set; } = string.Empty;
        public string TicketSectorName { get; set; } = string.Empty;
    }
}
