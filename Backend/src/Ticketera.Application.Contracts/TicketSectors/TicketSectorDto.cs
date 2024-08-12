using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Ticketera.TicketSectors
{
    public class TicketSectorDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatorId { get; set; }
    }
}
