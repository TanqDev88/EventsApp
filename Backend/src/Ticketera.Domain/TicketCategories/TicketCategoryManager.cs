using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Ticketera.Localization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;

namespace Ticketera.TicketCategories
{
    public class TicketCategoryManager : DomainService
    {
        private readonly IRepository<TicketCategory, long> _ticketCategoryRepository;
        private readonly IStringLocalizer<TicketeraResource> _localizer;
        private readonly ICurrentUser _currentUser;

        public TicketCategoryManager(IRepository<TicketCategory, long> ticketCategoryRepository,
            IStringLocalizer<TicketeraResource> localizer,
            ICurrentUser currentUser)
        {
            _ticketCategoryRepository = ticketCategoryRepository;
            _localizer = localizer;
            _currentUser = currentUser;
        }

        public async Task ValidateTicketCategoryAsync(string name, long? id = null)
        {
            var userId = _currentUser.GetId();

            var existingTicketCategory = await _ticketCategoryRepository.FirstOrDefaultAsync(x =>
                x.Name.ToLower() == name.ToLower() &&
                x.CreatorId == userId &&
                (id == null || x.Id != id)
            );

            if (existingTicketCategory != null)
            {
                throw new UserFriendlyException(_localizer["ExistTicketCategory", name]);
            }
        }
    }
}
