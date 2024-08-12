using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Ticketera.Entities;
using Ticketera.Localization;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Uow;

namespace Ticketera.BackgroundJobs
{
    public class SendingValidatedTicketByEmailBackgroundJob: AsyncBackgroundJob<SendingValidatedTicketByEmailBackgroundJobArgs>, ITransientDependency
    {
        private readonly IRepository<Ticket, long> _ticketRepository;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<TicketeraResource> _localizer;
        private readonly IConfiguration _configuration;


        public SendingValidatedTicketByEmailBackgroundJob(IEmailSender emailSender, IRepository<Ticket, long> ticketRepository, IStringLocalizer<TicketeraResource> localizer, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _ticketRepository = ticketRepository;
            _localizer = localizer;
            _configuration = configuration;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(SendingValidatedTicketByEmailBackgroundJobArgs args)
        {
            var query = await _ticketRepository.WithDetailsAsync(x => x.Purchase , x => x.TicketCategory, x => x.TicketSector);
            var ticket = query.FirstOrDefault(x => x.Id == args.TicketId);


            if (ticket == null)
            {
                throw new UserFriendlyException(_localizer["ErrorTicket"]);
            }

            try
            {
                string validatedTicket = Resource.validated_ticket;
                string body = validatedTicket
                    .Replace("{{validatedTicket}}", _localizer["ValidatedTicket"])
                    .Replace("{{nameEvent}}", ticket.Title)
                    .Replace("{{infoTicket}}", "1 x " + ticket.TicketCategory.Name + " - " + ticket.TicketSector.Name)
                    .Replace("{{text9}}", _localizer["Text9"])
                    .Replace("{{text10}}", _localizer["Text10"])
                    .Replace("{{qrValidated}}", $"{_configuration["App:SelfUrl"]}/resources/QrValidate.png")
                    .Replace("{{logoTixGo}}", $"{_configuration["App:SelfUrl"]}/resources/LogoTixGo.png")
                    .Replace("{{facebookIcon}}", $"{_configuration["App:SelfUrl"]}/resources/FacebookIcon.png")
                    .Replace("{{instagramIcon}}", $"{_configuration["App:SelfUrl"]}/resources/InstagramIcon.png")
                    .Replace("{{twitterIcon}}", $"{_configuration["App:SelfUrl"]}/resources/TwitterIcon.png");

                var subject = _localizer["PurchaseConfirmation"];

                await _emailSender.SendAsync(
                    ticket.Purchase.Email,
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
