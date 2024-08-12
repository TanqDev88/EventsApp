using Volo.Abp.Application.Dtos;

namespace Ticketera.ProviderPayments
{
    public class GetProviderPaymentListDto : PagedResultRequestDto
    {
        public string? Keyword { get; set; }
    }

}
