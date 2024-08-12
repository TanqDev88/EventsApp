using System;
using System.Collections.Generic;
using Ticketera.Base;
using Ticketera.Enum;

namespace Ticketera.Events
{
    public class EventDto: BaseAuditedDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public EventType EventType { get; set; } = EventType.Public;
        public string Code { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;


        public string PhotoGallery { get; set; } = string.Empty;
        public string PhotoDetail { get; set; } = string.Empty;
        public string PhotoLogo { get; set; } = string.Empty;
        public string IdPhotoLogo {  get; set; } = string.Empty;


        public int TicketsCount { get; set; }
        public int EventDatesCount { get; set; }
        public int UserEventsCount { get; set; }
        public int FileAttachmentsCount { get; set; }

        public bool IsOwner { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public bool IsEditor { get; set; } = false;
        public bool IsValidator { get; set; } = false;
        public bool IsActive { get; set; } = false;

        public bool IsMain { get; set; }
        public EventStatus EventStatus { get; set; } = EventStatus.Available;
        public string Place { get; set; } = string.Empty;
        public IEnumerable<EventDateDto> EventDates { get; set; } = new List<EventDateDto>();
        public IList<PriceDto> Prices { get; set;} = new List<PriceDto>();
        public IList<long> IdProviderPayment {  get; set; } = new List<long>();
        public int SaleForPerson { get; set; }
        public IEnumerable<UserEventDto> UserEvents { get; set; } = new List<UserEventDto>();
    }
}
