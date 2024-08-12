using Hangfire;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading;
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
    public class TicketRefreshBackgroundJob : AsyncBackgroundJob<EventRefreshTicketBackgroundJobArgs>, ITransientDependency
    {
        private readonly IRepository<Event,long> _eventRepository;
        private readonly IRepository<EventDate, long> _eventDateRepository;
        private readonly IRepository<Ticket,long> _ticketRepository;
        private readonly IHubContext<EventHub> _hubContext;

        public TicketRefreshBackgroundJob(IRepository<Ticket, long> ticketRepository, IHubContext<EventHub> hubContext, IRepository<Event, long> eventRepository, IRepository<EventDate, long> eventDateRepository)
        {
            _ticketRepository = ticketRepository;
            _hubContext = hubContext;
            _eventRepository = eventRepository;
            _eventDateRepository = eventDateRepository;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(EventRefreshTicketBackgroundJobArgs args)
        {
            var eventDateId = args.EventDateId;
            var eventDate = await _eventDateRepository.FirstOrDefaultAsync(x => x.Id == eventDateId);

            if (eventDate == null) return;

            var ev = await _eventRepository.FirstOrDefaultAsync(x => x.Id == eventDate.EventId);
            
            if (ev != null)
            {
                var eventDates = await _eventDateRepository.GetListAsync(x => x.EventId == ev.Id);
                var lastEventDate = eventDates.OrderByDescending(x => x.StartDate).FirstOrDefault();

                // Check if eventDate is the last date of the event
                var isLastDate = eventDate.Id == lastEventDate?.Id;

                // -- Check if date actuality equal today
                var date = DateTime.Now;
                var dateNowEnd = eventDate.EndDate.HasValue ? eventDate.EndDate.Value.AddHours(6) : eventDate.StartDate.AddHours(6);
                
                var query = await _ticketRepository.GetQueryableAsync();
                var tickets = query.Where(x => x.EventDateId == eventDateId && 
                                               x.TicketStatus == Enum.TicketStatus.Sold)
                                   .ToList();

                // -- Exist tickets
                if (tickets.Any())
                {
                    // -- Regenerate Code to Ticket
                    await _ticketRepository.UpdateManyAsync(tickets.Select(x =>
                    {
                        x.Code = Guid.NewGuid().ToString();
                        return x;
                    }), true);

                    // -- Notificatión for refresh image
                    await _hubContext.Clients.All.SendAsync(ev.Code + "-" + eventDate.StartDate.ToString("yyMMddHHmm"));
                }

                if (date <= dateNowEnd)
                {
                    var delay = date.AddSeconds(15);
                    BackgroundJob.Schedule(() => BackgroundJob.Requeue(eventDate.BackgroundJobId), delay);
                }
                else if(date > dateNowEnd && isLastDate) 
                {
                    ev.EventStatus = EventStatus.Finalized;
                    BackgroundJob.Delete(ev.BackgroundJobId);
                    await _eventRepository.UpdateAsync(ev, true);
                    await _hubContext.Clients.All.SendAsync("EventStatusCode", ev.Code);
                    await _hubContext.Clients.All.SendAsync("EventStatus", ev.EventStatus);
                }
            }
        }
    }
}
