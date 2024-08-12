using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Ticketera.Localization;
using Ticketera.ProviderPayments;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;

namespace Ticketera.ProviderPayments
{
    public class ProviderPaymentManager : DomainService
    {
        private readonly IRepository<ProviderPayment, long> _providerPaymentRepository;
        private readonly IStringLocalizer<TicketeraResource> _localizer;
        private readonly ICurrentUser _currentUser;

        public ProviderPaymentManager(IRepository<ProviderPayment, long> providerPaymentRepository,
            IStringLocalizer<TicketeraResource> localizer,
            ICurrentUser currentUser)
        {
            _providerPaymentRepository = providerPaymentRepository;
            _localizer = localizer;
            _currentUser = currentUser;
        }

        public async Task ValidateProviderPaymentAsync(string name, long? id = null)
        {
            var userId = _currentUser.GetId();

            var existingProviderPayment = await _providerPaymentRepository.FirstOrDefaultAsync(x =>
                                                             x.Name.ToLower() == name.ToLower() &&
                                                             x.CreatorId == userId &&
                                                             (id == null || x.Id != id));

            if (existingProviderPayment != null)
            {
                throw new UserFriendlyException(_localizer["ExistProviderPayment", name]);
            }
        }
    }
}
