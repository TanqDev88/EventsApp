using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Ticketera.ProviderPayments
{
    public interface IProviderPaymentAppService : ICrudAppService<ProviderPaymentDto, long, GetProviderPaymentListDto>
    {
    }
}
