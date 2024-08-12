using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ticketera.Entities;
using Ticketera.Enum;
using Ticketera.Events;
using Ticketera.Localization;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Uow;

namespace Ticketera.BackgroundJobs
{
    public class CancelPurchaseBackgroundJob : AsyncBackgroundJob<CancelPurchaseBackgroundJobArgs>, ITransientDependency
    {
        private readonly IRepository<Purchase, long> _purchaseRepo;
        private readonly IRepository<Ticket, long> _ticketRepo;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IStringLocalizer<TicketeraResource> _localizer;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public CancelPurchaseBackgroundJob(IRepository<Purchase, long> purchaseRepo, IRepository<Ticket, long> ticketRepo, IHubContext<EventHub> hubContext, IBackgroundJobManager backgroundJobManager, IStringLocalizer<TicketeraResource> localizer, IEmailSender emailSender, IConfiguration configuration)
        {
            _purchaseRepo = purchaseRepo;
            _ticketRepo = ticketRepo;
            _hubContext = hubContext;
            _backgroundJobManager = backgroundJobManager;
            _localizer = localizer;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(CancelPurchaseBackgroundJobArgs args)
        {
            try
            {
                await CancelPurchase(args.Code, args.State);
            }
            catch (Exception ex)
            {

            }

        }

        private async Task CancelPurchase(string code, PurchaseState state)
        {
            // -- Check if exist purchase inProcess
            var query = await _purchaseRepo.WithDetailsAsync();
            var purchase = query.FirstOrDefault(x => x.Code == code && x.State == state);
            if (purchase != null)
            {
                if(purchase.State == PurchaseState.InProcess)
                {
                    // -- Get event id
                    var eventIds = purchase.Ticktes.Select(x => x.EventId).Distinct().ToList();

                    // -- Delete tickets and purchase reserved
                    await _ticketRepo.DeleteAsync(x => x.PurchaseId == purchase.Id);
                    await _purchaseRepo.DeleteAsync(purchase.Id, true);

                    // -- Notificatión when tickets reserved
                    await _hubContext.Clients.All.SendAsync(TicketeraConsts.NotificationTicket, string.Join('|', eventIds));

                    // -- Send email expired purchase
                    await SendEmailExpiredPurchase(purchase);
                }

                if(purchase.State == PurchaseState.Pending)
                {
                    // -- Create backgroundJob to delete purchase                        
                    await _backgroundJobManager.EnqueueAsync(new CancelPurchasePendingBackgroundJobArgs
                    {
                        Code = purchase.Code,
                        State = PurchaseState.Finish,
                    }, delay: TimeSpan.FromDays(3));
                }
            }
        }

        private async Task SendEmailExpiredPurchase(Purchase purchase)
        {
            try
            {
                string purchaseConfirmationHtml = Resource.expired_purchase;

                string body = purchaseConfirmationHtml
                    .Replace("{{purchaseExpired}}", _localizer["PurchaseExpired"])
                    .Replace("{{descriptionPurchaseExpired}}", _localizer["DescriptionPurchaseExpired"])
                    .Replace("{{urlEvents}}", $"{_configuration["App:ClientUrl"]}/events")
                    .Replace("{{tixgo}}", _localizer["tixgo.mx"])
                    .Replace("{{text9}}", _localizer["Text9"])
                    .Replace("{{text10}}", _localizer["Text10"])
                    .Replace("{{logoTixGo}}", $"{_configuration["App:SelfUrl"]}/resources/LogoTixGo.png")
                    .Replace("{{imageSadFace}}", $"{_configuration["App:SelfUrl"]}/resources/SadFace.png")
                    .Replace("{{facebookIcon}}", $"{_configuration["App:SelfUrl"]}/resources/FacebookIcon.png")
                    .Replace("{{instagramIcon}}", $"{_configuration["App:SelfUrl"]}/resources/InstagramIcon.png")
                    .Replace("{{twitterIcon}}", $"{_configuration["App:SelfUrl"]}/resources/TwitterIcon.png");
                var subject = _localizer["PurchaseExpired"] + ".";

                await _emailSender.SendAsync(
                    purchase.Email,
                    subject,
                    body,
                    true
                );
            }
            catch (Exception)
            {
                throw new UserFriendlyException(_localizer["ErrorEmail"]);
            }
        }
    }
}
