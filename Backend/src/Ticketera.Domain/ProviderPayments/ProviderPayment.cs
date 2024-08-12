using System;
using System.Collections.Generic;
using Ticketera.Entities;

namespace Ticketera.ProviderPayments
{
    public class ProviderPayment : Base
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatorId { get; set; }
        public virtual IEnumerable<EventProviderPayment> EventProviders { get; set; } = new List<EventProviderPayment>();
    }
}
