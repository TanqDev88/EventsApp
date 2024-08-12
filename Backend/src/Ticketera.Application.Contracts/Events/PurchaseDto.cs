using Ticketera.Enum;

namespace Ticketera.Events
{
    public class PurchaseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public PurchaseState State { get; set; } = PurchaseState.Pending;
    }
}
