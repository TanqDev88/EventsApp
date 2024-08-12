using System;

namespace Ticketera.Tickets
{
    public class PurchaseInputDto
    {
        public string Code { get; set; }
        public long EventId { get; set; }
        public long EventDateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
