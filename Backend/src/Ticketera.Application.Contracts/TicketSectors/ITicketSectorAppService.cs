using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Ticketera.TicketSectors
{
    public interface ITicketSectorAppService : ICrudAppService<TicketSectorDto, long, GetTicketSectorListDto>
    {
    }
}
