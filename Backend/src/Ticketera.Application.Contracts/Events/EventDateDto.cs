using System;

namespace Ticketera.Events
{
    public class EventDateDto
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public long EventId { get; set; }
    }
}
