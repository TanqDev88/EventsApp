namespace Ticketera.Entities
{
    public class PriceModel
    {
        public int Count { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        public long TicketCategoryId { get; set; }
        public string TicketCategoryName { get; set; }

        public long TicketSectorId { get; set; }
        public string TicketSectorName { get; set; }
    }
}
