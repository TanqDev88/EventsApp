using System;
using System.Collections.Generic;
using System.Text;
using Ticketera.Base;

namespace Ticketera.Tickets
{
    public class TicketDto : BaseAuditedDto
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public string TicketCategoryName { get; set; }
    }
}
