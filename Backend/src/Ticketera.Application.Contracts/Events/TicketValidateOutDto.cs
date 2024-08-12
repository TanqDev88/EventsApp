using Ticketera.Enum;

namespace Ticketera.Events
{
    public class TicketValidateOutDto
    {
        public string ClientName { get; set; }
        public string CategoryName { get; set; }
        public string SectorName { get; set; }
        public TicketValidateType Status { get; set; }
    }
}
