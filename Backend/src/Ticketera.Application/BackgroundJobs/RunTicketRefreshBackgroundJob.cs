using Nito.AsyncEx;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Ticketera.Events;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Ticketera.BackgroundJobs
{
    public class RunTicketRefreshBackgroundJob : AsyncBackgroundJob<DateTime>, ITransientDependency
    {
        private readonly IRepository<EventDate, long> _eventDateRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public RunTicketRefreshBackgroundJob(IRepository<EventDate, long> eventDateRepository, IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
            _eventDateRepository = eventDateRepository;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(DateTime arg)
        {
            // -- Get all event date for today
            var query = await _eventDateRepository.GetQueryableAsync();
            // -- Filter 10 hours after and before
            var dateStart = new DateTime(arg.Year, arg.Month, arg.Day, 0, 0, 0).AddHours(-10);
            var dateEnd = new DateTime(arg.Year, arg.Month, arg.Day, 23, 59, 59).AddHours(10);
            var evenDateIds = query.Where(x => x.StartDate >= dateStart &&
                                              (
                                                (x.EndDate.HasValue && x.EndDate.Value <= dateEnd) ||
                                                (!x.EndDate.HasValue && x.StartDate <= dateEnd)
                                              ))
                                   .OrderBy(x => x.StartDate)
                                   .ToList();

            // -- If exist event date run backgroundjob to refresh Code for tickets
            if (!evenDateIds.Any()) return;

            var dates = await evenDateIds.Select(async eventDate =>
            {

                var dateInit = eventDate.StartDate.AddHours(-10);
                var delay = dateInit - arg;

                var jobId = await _backgroundJobManager.EnqueueAsync(new EventRefreshTicketBackgroundJobArgs
                {
                    EventDateId = eventDate.Id
                }, delay: delay);

                eventDate.BackgroundJobId = jobId;

                return eventDate;
            }).WhenAll();

            await _eventDateRepository.UpdateManyAsync(dates, true);
        }
    }
}
