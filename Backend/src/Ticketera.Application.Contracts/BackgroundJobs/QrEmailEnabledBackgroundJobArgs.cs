namespace Ticketera.BackgroundJobs
{
    public class QrEmailEnabledBackgroundJobArgs
    {
        public long EventId { get; set; }
        public long PurchaseId { get; set; }
        public long EventDateId { get; set; }
    }
}
