using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ticketera.Entities;
using Ticketera.Events;
using Ticketera.Localization;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Localization;
using Volo.Abp.Uow;


namespace Ticketera.BackgroundJobs
{
    public class QrEmailSendingEnabledBackgroundJob : AsyncBackgroundJob<QrEmailEnabledBackgroundJobArgs>, ITransientDependency
    {
        private readonly IRepository<Event, long> _eventRepository;
        private readonly IRepository<Purchase, long> _purchaseRepository;
        private readonly IRepository<EventDate, long> _eventDateRepository;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<TicketeraResource> _localizer;
        private readonly IConfiguration _configuration;

        public QrEmailSendingEnabledBackgroundJob(IRepository<Event, long> eventRepository, IRepository<Purchase, long> purchaseRepository, IEmailSender emailSender, IStringLocalizer<TicketeraResource> localizer, IConfiguration configuration, IRepository<EventDate, long> eventDateRepository)
        {
            _eventRepository = eventRepository;
            _purchaseRepository = purchaseRepository;
            _emailSender = emailSender;
            _localizer = localizer;
            _configuration = configuration;
            _eventDateRepository = eventDateRepository;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(QrEmailEnabledBackgroundJobArgs args)
        {
            try
            {
                // TODO : Solve lenguage to not set fixed language 
                CultureHelper.Use("es-ES");

                var eventDate = await _eventDateRepository.FirstOrDefaultAsync(x => x.Id == args.EventDateId);
                var eve = await _eventRepository.FirstOrDefaultAsync(x => x.Id == args.EventId);

                var queryTickets = await _purchaseRepository.WithDetailsAsync(x => x.Ticktes);
                var purchaseTickets = queryTickets.First(x => x.Id == args.PurchaseId);

                var mercadoPagoResponseObject = JsonConvert.DeserializeObject<MercadoPago.Resource.Payment.Payment>(purchaseTickets.MercadoPagoResponse);
                decimal? transactionAmount = mercadoPagoResponseObject?.TransactionAmount;

                string listTickets = Resource.list_tickets;
                string ticketRows = "";

                foreach (var ticket in purchaseTickets.Ticktes)
                {
                    var ticketCategory = eve.Prices.FirstOrDefault(p => p.TicketCategoryId == ticket.TicketCategoryId);
                    var ticketSector = eve.Prices.FirstOrDefault(p => p.TicketSectorId == ticket.TicketSectorId);
                    var urlTicket = $"{_configuration["App:ClientUrl"]}/event/{eve.Code}/purchase/{purchaseTickets.Code}/{ticket.Id}";

                    string row = listTickets
                        .Replace("{{infoTicket}}", "1 x " + ticketCategory.TicketCategoryName + " - " + ticketSector.TicketSectorName)
                        .Replace("{{text11}}", _localizer["Text11"])
                        .Replace("{{urlTicket}}", urlTicket);
                    ticketRows += row;
                }

                string purchaseConfirmationHtml = Resource.ticket_enable;
                string body = purchaseConfirmationHtml
                    .Replace("{{text1}}", _localizer["Text1"])
                    .Replace("{{nameEvent}}", eve.Name)
                    .Replace("{{list}}", ticketRows)
                    .Replace("{{text4}}", _localizer["Text4"])
                    .Replace("{{fullPayment}}", "$ " + transactionAmount.ToString())
                    .Replace("{{text5}}", _localizer["Place"])
                    .Replace("{{place}}", eve.Place)
                    .Replace("{{text6}}", _localizer["Text6"])
                    .Replace("{{dateEvent}}", eventDate.StartDate.ToString("dd/MM/yyyy"))
                    .Replace("{{text7}}", _localizer["Text7"])
                    .Replace("{hourEvent}", eventDate.StartDate.ToString("HH:mm"))
                    .Replace("{{text9}}", _localizer["Text9"])
                    .Replace("{{text10}}", _localizer["Text10"])
                    .Replace("{{logoTixGo}}", $"{_configuration["App:SelfUrl"]}/resources/LogoTixGo.png")
                    .Replace("{{facebookIcon}}", $"{_configuration["App:SelfUrl"]}/resources/FacebookIcon.png")
                    .Replace("{{instagramIcon}}", $"{_configuration["App:SelfUrl"]}/resources/InstagramIcon.png")
                    .Replace("{{twitterIcon}}", $"{_configuration["App:SelfUrl"]}/resources/TwitterIcon.png");

                var subject = _localizer["PurchaseConfirmation"];

                await _emailSender.SendAsync(
                    purchaseTickets.Email,
                    subject,
                    body,
                    true
                );
            }
            catch (Exception ex)
            {
                Logger.LogError(_localizer["EmailError"], ex);
            }
        }
    }
}
