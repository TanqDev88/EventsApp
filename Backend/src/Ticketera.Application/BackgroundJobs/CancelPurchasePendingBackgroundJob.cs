using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ticketera.Entities;
using Ticketera.Enum;
using Ticketera.Events;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Ticketera.BackgroundJobs
{
    public class CancelPurchasePendingBackgroundJob : AsyncBackgroundJob<CancelPurchasePendingBackgroundJobArgs>, ITransientDependency
    {
        private readonly IRepository<Purchase, long> _purchaseRepo;
        private readonly IRepository<Ticket, long> _ticketRepo;
        private readonly IHubContext<EventHub> _hubContext;

        public CancelPurchasePendingBackgroundJob(IRepository<Purchase, long> purchaseRepo, IRepository<Ticket, long> ticketRepo, IHubContext<EventHub> hubContext)
        {
            _purchaseRepo = purchaseRepo;
            _ticketRepo = ticketRepo;
            _hubContext = hubContext;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(CancelPurchasePendingBackgroundJobArgs args)
        {
                await CancelPurchase(args.Code, args.State);
        }

        private async Task CancelPurchase(string code, PurchaseState state)
        {
            // -- Check if exist purchase inProcess
            var query = await _purchaseRepo.WithDetailsAsync();
            var purchase = query.FirstOrDefault(x => x.Code == code);
            if (purchase != null && purchase.State != state)
            {
                // -- Get event id
                var eventIds = purchase.Ticktes.Select(x => x.EventId).Distinct().ToList();

                // -- Delete tickets and purchase reserved
                await _ticketRepo.DeleteAsync(x => x.PurchaseId == purchase.Id);
                await _purchaseRepo.DeleteAsync(purchase.Id, true);

                // -- Notificatión when tickets reserved
                await _hubContext.Clients.All.SendAsync(TicketeraConsts.NotificationTicket, string.Join('|', eventIds));
            }
        }
    }
}
