using System;
using System.Collections.Generic;

namespace Ticketera.Events
{
    public class EventFilterDto
    {
        public string Keyword { get; set; } = string.Empty;
        public Guid? CreatorId { get; set; }
        public IList<long> ExcludeIds { get; set; } = new List<long>();
        public int Skip { get; set; } = 0;
        public int Size { get; set; } = 8;
        public DateTime? StartDate { get; set; }
    }
}
