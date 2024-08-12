using Hangfire;
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
    public class DashboardStatisticsBackgroundJob : AsyncBackgroundJob<DashboardStatisticsBackgroundJobArgs>, ITransientDependency
    {
        private readonly IRepository<Event, long> _eventRepository;
        private readonly IRepository<Ticket, long> _ticketRepo;
        private readonly IHubContext<EventHub> _hubContext;

        public DashboardStatisticsBackgroundJob(IRepository<Event, long> eventRepository, IRepository<Ticket, long> ticketRepo, IHubContext<EventHub> hubContext)
        {
            _eventRepository = eventRepository;
            _ticketRepo = ticketRepo;
            _hubContext = hubContext;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(DashboardStatisticsBackgroundJobArgs args)
        {
            var today = DateTime.Today;
            var totalActiveEvents = 0;
            var totalSoldTickets = 0;
            var totalRevenue = 0.0m;
            var ticketsSoldToday = 0;

            // Get events with UserEvents details
            var eventsWithDetails = await _eventRepository.WithDetailsAsync(x => x.UserEvents);

            // Filter events where CurrentUser.Id is the creator and event status is available or sold out
            var userEvents = eventsWithDetails.Where(e => e.CreatorId == args.CurrentUserId
                                                           && (e.EventStatus == EventStatus.Available || e.EventStatus == EventStatus.SoldOut)).ToList();

            // Filter active events and count them
            totalActiveEvents = userEvents.Count(e => e.IsActive);

            // Get IDs of active events
            var activeEventIds = userEvents.Where(e => e.IsActive).Select(e => e.Id).ToList();

            if (!activeEventIds.Any())
            {
                totalActiveEvents = 0;
            }

            // Get and filter sold tickets from active events
            var soldTickets = await _ticketRepo.GetListAsync(t =>
                activeEventIds.Contains(t.EventId) &&
                t.TicketStatus == TicketStatus.Sold);

            if (soldTickets == null || !soldTickets.Any())
            {
                // Handle case where no tickets are sold
                totalSoldTickets = 0;
            }
            else
            {
                // Count sold tickets and calculate sum of prices
                totalSoldTickets = soldTickets.Count;
                totalRevenue = soldTickets.Sum(t => t.Price);
            }

            // Get all sold tickets for events with required status where CurrentUser.Id is the creator
            var allSoldTickets = await _ticketRepo.GetListAsync(t =>
                userEvents.Select(e => e.Id).Contains(t.EventId) &&
                t.TicketStatus == TicketStatus.Sold);

            // Filter and count tickets sold today from all user events with required status
            ticketsSoldToday = allSoldTickets.Count(t =>
                t.CreationTime.AddHours(-3) >= today && t.CreationTime.AddHours(-3) < today.AddDays(1));

            // Create DashboardStatisticsDto object with calculated statistics
            var statistics = new DashboardStatisticsDto
            {
                TotalActiveEvents = totalActiveEvents,
                TotalTicketsSold = totalSoldTickets,
                TotalRevenue = totalRevenue,
                TicketsSoldToday = ticketsSoldToday
            };

            // Send statistics to all clients using SignalR hub
            await _hubContext.Clients.All.SendAsync("StatisticsEvents", statistics);
        }
    }
}
