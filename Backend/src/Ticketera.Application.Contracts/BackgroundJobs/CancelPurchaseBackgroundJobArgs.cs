using System;
using Ticketera.Enum;

namespace Ticketera.BackgroundJobs
{
    public class CancelPurchaseBackgroundJobArgs
    {
        public string Code { get; set; }
        public TimeSpan? Delay { get; set; }
        public PurchaseState State { get; set; } = PurchaseState.Pending;
    }
}
