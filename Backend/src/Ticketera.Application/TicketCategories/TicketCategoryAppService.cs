
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;


namespace Ticketera.TicketCategories
{
    public class TicketCategoryAppService : CrudAppService<TicketCategory, TicketCategoryDto, long, GetTicketCategoryListDto>, ITicketCategoryAppService
    {
        private readonly TicketCategoryManager _ticketCategoryManager;
        public TicketCategoryAppService(IRepository<TicketCategory, long> repository,
            TicketCategoryManager ticketCategoryManager
            )
            : base(repository)
        {
            _ticketCategoryManager = ticketCategoryManager;
        }

        public override async Task<TicketCategoryDto> CreateAsync(TicketCategoryDto input)
        {
            await _ticketCategoryManager.ValidateTicketCategoryAsync(input.Name);
            input.CreatorId = CurrentUser.Id.Value;
            return await base.CreateAsync(input);
        }

        public override async Task<TicketCategoryDto> UpdateAsync(long id, TicketCategoryDto input)
        {
            await _ticketCategoryManager.ValidateTicketCategoryAsync(input.Name, id);
            input.CreatorId = CurrentUser.Id.Value;
            return await base.UpdateAsync(id, input);
        }


        public override async Task<PagedResultDto<TicketCategoryDto>> GetListAsync(GetTicketCategoryListDto input)
        {
            //Get the IQueryable<categories> from the repository
            var queryable = await Repository.GetQueryableAsync();

            //Get categories
            var ticketCategories = await AsyncExecuter.ToListAsync(
                    queryable
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.ToLower().Contains(input.Keyword.ToLower()) && x.CreatorId == CurrentUser.Id)
                        .Skip(input.SkipCount)
                        .Take(input.MaxResultCount)
            );

            //Convert to DTOs
            var listCategories = ObjectMapper.Map<List<TicketCategory>, List<TicketCategoryDto>>(ticketCategories);

            var totalCount = await Repository.GetCountAsync();

            return new PagedResultDto<TicketCategoryDto>(totalCount, listCategories);
        }

    }
}
