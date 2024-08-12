using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketera.Events;
using Ticketera.Localization;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Ticketera.BackgroundJobs
{
    public class UpdateEventDatesBackgroundJob : AsyncBackgroundJob<UpdateEventDatesBackgroundJobArgs>, ITransientDependency
    {
        private readonly IRepository<Event, long> _eventRepository;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly IStringLocalizer<TicketeraResource> _localizer;
        public UpdateEventDatesBackgroundJob(IHubContext<EventHub> hubContext, IRepository<Event, long> eventRepository, IStringLocalizer<TicketeraResource> localizer)
        {
            _hubContext = hubContext;
            _eventRepository = eventRepository;
            _localizer = localizer;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(UpdateEventDatesBackgroundJobArgs args)
        {
            var query = await _eventRepository.WithDetailsAsync(x => x.EventDates);
            var eve = query.First(x => x.Id == args.EventId);

            if (eve == null)
            {
                throw new UserFriendlyException(_localizer["NonExistentEvent"]);
            }

            // Calculate remaining days until the nearest event date
            var nearestEventDate = eve.EventDates
                .Where(ed => ed.StartDate >= DateTime.UtcNow)
                .OrderBy(ed => ed.StartDate)
                .FirstOrDefault();

            int daysRemaining = nearestEventDate != null ? (nearestEventDate.StartDate - DateTime.Now).Days : 0;

            // Calculate days the event has been on sale
            int daysOnSale = (DateTime.UtcNow - eve.CreationTime).Days;

            var eventDays = new EventDaysDto
            {
                DaysRemaining = daysRemaining,
                DaysOnSale = daysOnSale,
            };

            // Send the update to all clients via SignalR
            await _hubContext.Clients.All.SendAsync("UpdateEventDates", eventDays);

            // Schedule the job to requeue at the next midnight
            var delay = DateTime.Today.AddDays(1).AddHours(0);
            BackgroundJob.Schedule(() => BackgroundJob.Requeue(eve.BackgroundJobId), delay);
        }
    }
}
