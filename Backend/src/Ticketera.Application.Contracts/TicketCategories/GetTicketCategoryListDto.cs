using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Ticketera.TicketCategories
{
    public class GetTicketCategoryListDto : PagedResultRequestDto
    {
        public string? Keyword { get; set; }
    }
}
