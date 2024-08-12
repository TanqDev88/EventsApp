using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ticketera.ProviderPayments
{
    public class ProviderPaymentAppService : CrudAppService<ProviderPayment, ProviderPaymentDto, long, GetProviderPaymentListDto>
    {
        private readonly ProviderPaymentManager _providerPaymentManager;
        public ProviderPaymentAppService(IRepository<ProviderPayment, long> repository,
            ProviderPaymentManager providerPaymentManager) : base(repository)
        {
            _providerPaymentManager = providerPaymentManager;
        }

        public override async Task<ProviderPaymentDto> CreateAsync(ProviderPaymentDto input)
        {
            await _providerPaymentManager.ValidateProviderPaymentAsync(input.Name);
            input.CreatorId = CurrentUser.Id.Value;
            return await base.CreateAsync(input);
        }

        public override async Task<ProviderPaymentDto> UpdateAsync(long id, ProviderPaymentDto input)
        {
            await _providerPaymentManager.ValidateProviderPaymentAsync(input.Name, id);
            input.CreatorId = CurrentUser.Id.Value;
            return await base.UpdateAsync(id, input);
        }

        public override async Task<PagedResultDto<ProviderPaymentDto>> GetListAsync(GetProviderPaymentListDto input)
        {
            var userId = CurrentUser.Id;
            var queryable = await Repository.GetQueryableAsync();

            var providerPayments = await AsyncExecuter.ToListAsync(
                    queryable
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.ToLower().Contains(input.Keyword.ToLower()) && x.CreatorId == userId)
                        .Skip(input.SkipCount)
                        .Take(input.MaxResultCount)
            );

            var listCategories = ObjectMapper.Map<List<ProviderPayment>, List<ProviderPaymentDto>>(providerPayments);

            var totalCount = await Repository.GetCountAsync();

            return new PagedResultDto<ProviderPaymentDto>(totalCount, listCategories);
        }
    }
}
