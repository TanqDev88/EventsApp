using System;
using System.Collections.Generic;
using Ticketera.Entities;

namespace Ticketera.Events
{
    public class EventDate : BaseAudited
    {
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public string BackgroundJobId { get; set; } = string.Empty;

        
        #region relations
        public long EventId { get; set; }
        public virtual Event Event { get; set; }
        #endregion


        #region lists
        public virtual IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>();
        #endregion
    }
}
