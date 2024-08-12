using System;
using System.Collections.Generic;
using Ticketera.Enum;
using Volo.Abp.Application.Dtos;

namespace Ticketera.Events
{
    public class EventResultRequestDto : PagedResultRequestDto
    {
        public Guid? UserId { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public EventType? EventType { get; set; }

        public bool IncludeFiles { get; set; } = false;
        public bool? IsMain { get; set; }
        public bool IsOwner { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public bool IsEditor { get; set; } = false;
        public bool IsValidator { get; set; } = false;
        public bool IsMobile { get; set; } = false;
        public EventStatus EventStatus { get; set; } = EventStatus.Available;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string Order { get; set; } = "desc";

        public IList<long> ExcludeIds { get; set; } = new List<long>();
    }
}
