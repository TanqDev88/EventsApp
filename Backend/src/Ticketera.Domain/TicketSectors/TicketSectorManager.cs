using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Ticketera.Entities;
using Ticketera.Localization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;
using Volo.Abp.Validation.Localization;

namespace Ticketera.TicketSectors
{
    public class TicketSectorManager : DomainService
    {
        private readonly IRepository<TicketSector, long> _ticketSectorRepository;
        private readonly IStringLocalizer<TicketeraResource> _localizer;
        private readonly ICurrentUser _currentUser;

        public TicketSectorManager(IRepository<TicketSector, long> ticketSectorRepository,
            IStringLocalizer<TicketeraResource> localizer,
            ICurrentUser currentUser)
        {
            _ticketSectorRepository = ticketSectorRepository;
            _localizer = localizer;
            _currentUser = currentUser;
        }

        public async Task ValidateTicketSectorAsync(string name, long? id = null)
        {
            var userId = _currentUser.GetId();

            var existingSector = await _ticketSectorRepository.FirstOrDefaultAsync(x =>
                x.Name.ToLower() == name.ToLower() &&
                x.CreatorId == userId &&
                (id == null || x.Id != id)
            );

            if (existingSector != null)
            {
                throw new UserFriendlyException(_localizer["ExistTicketSector", name]);
            }
        }
    }
}