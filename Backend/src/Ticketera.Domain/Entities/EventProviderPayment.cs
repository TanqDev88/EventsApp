using Ticketera.Events;
using Ticketera.ProviderPayments;

namespace Ticketera.Entities
{
    public class EventProviderPayment : Base
    {
        public long EventId { get; set; }
        public virtual Event Event { get; set; }
        public long ProviderPaymentId { get; set; }
        public virtual ProviderPayment ProviderPayment { get; set; }
    }
}
