using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ticketera.TicketSectors
{
    public class TicketSectorAppService : CrudAppService<TicketSector, TicketSectorDto, long, GetTicketSectorListDto>, ITicketSectorAppService
    {
        private readonly TicketSectorManager _ticketSectorManager;
        public TicketSectorAppService(IRepository<TicketSector, long> repository,
            TicketSectorManager ticketSectorManager) : base(repository)
        {
            _ticketSectorManager = ticketSectorManager;
        }

        public override async Task<TicketSectorDto> CreateAsync(TicketSectorDto input)
        {
            await _ticketSectorManager.ValidateTicketSectorAsync(input.Name);
            input.CreatorId = CurrentUser.Id.Value;
            return await base.CreateAsync(input);
        }

        public override async Task<TicketSectorDto> UpdateAsync(long id, TicketSectorDto input)
        {
            await _ticketSectorManager.ValidateTicketSectorAsync(input.Name, id);
            input.CreatorId = CurrentUser.Id.Value;
            return await base.UpdateAsync(id, input);
        }


        public override async Task<PagedResultDto<TicketSectorDto>> GetListAsync(GetTicketSectorListDto input)
        {
            var queryable = await Repository.GetQueryableAsync();

            var ticketSectors = await AsyncExecuter.ToListAsync(
                    queryable
                        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.ToLower().Contains(input.Keyword.ToLower()) && x.CreatorId == CurrentUser.Id)
                        .Skip(input.SkipCount)
                        .Take(input.MaxResultCount)
            );

            var listSectors = ObjectMapper.Map<List<TicketSector>, List<TicketSectorDto>>(ticketSectors);

            var totalCount = await Repository.GetCountAsync();

            return new PagedResultDto<TicketSectorDto>(totalCount, listSectors);
        }
    }
}
