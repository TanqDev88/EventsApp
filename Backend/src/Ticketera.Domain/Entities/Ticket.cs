using System;
using Ticketera.Enum;
using Ticketera.Events;
using Ticketera.TicketCategories;
using Ticketera.TicketSectors;
using Volo.Abp.Data;

namespace Ticketera.Entities
{
    public class Ticket : BaseAudited, IHasExtraProperties
    {
        public string Title { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public ExtraPropertyDictionary ExtraProperties { get; } = new ExtraPropertyDictionary();

        #region enums
        public CodeType CodeType { get; set; } = CodeType.Qr;
        public TicketStatus TicketStatus { get; set; } = TicketStatus.Reserved;
        #endregion

        #region Relations
        public long EventId { get; set; }
        public virtual Event Event { get; set; }

        public long EventDateId { get; set; }
        public virtual EventDate EventDate { get; set; }

        public long PurchaseId { get; set; }
        public virtual Purchase Purchase { get; set; }

        public long? TicketCategoryId { get; set; }
        public virtual TicketCategory TicketCategory { get; set; }

        public long? TicketSectorId { get; set; }
        public virtual TicketSector TicketSector { get; set; }

        #endregion

    }
}
