using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Ticketera.TicketCategories
{
    public interface ITicketCategoryAppService : ICrudAppService<TicketCategoryDto, long, GetTicketCategoryListDto>
    {
    }
}
