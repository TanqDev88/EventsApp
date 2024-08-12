using System;
using System.Collections.Generic;
using Ticketera.Base;
using Ticketera.Enum;

namespace Ticketera.Events
{
    public class EventInputDto : BaseAuditedDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public EventType EventType { get; set; } = EventType.Public;
        public string Code { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;


        public long PhotoGallery { get; set; }
        public long PhotoDetail { get; set; }
        public long PhotoLogo { get; set; }


        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public EventStatus EventStatus { get; set; } = EventStatus.Available;
        public string Place { get; set; } = string.Empty;
        public IList<EventDateDto> EventDatesInput { get; set; } = new List<EventDateDto>();
        public IList<PriceDto> Prices { get; set; } = new List<PriceDto>();
        public IList<long> IdProviderPayment { get; set; } = new List<long>();
        public Dictionary<Guid, TypeUserEvent> Validators { get; set; } = new Dictionary<Guid, TypeUserEvent>();
        public int SaleForPerson { get; set; }
    }
}
