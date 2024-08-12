using Volo.Abp.Application.Dtos;

namespace Ticketera.TicketSectors
{
    public class GetTicketSectorListDto : PagedResultRequestDto
    {
        public string? Keyword { get; set; }
    }
}
