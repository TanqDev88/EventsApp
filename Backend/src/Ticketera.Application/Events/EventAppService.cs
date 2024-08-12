using Microsoft.AspNetCore.Authorization;
using Ticketera.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketera.Entities;
using Ticketera.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Ticketera.Enum;
using Ticketera.Storages;
using Microsoft.AspNetCore.SignalR;
using Ticketera.Tickets;
using System.Net.Sockets;
using Volo.Abp.BackgroundJobs;
using MercadoPago.Config;
using Microsoft.Extensions.Configuration;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using MercadoPago.Client.Common;
using Newtonsoft.Json;
using MercadoPago.Client.Payment;
using System.Text.RegularExpressions;
using Volo.Abp.Emailing;
using Volo.Abp.Localization;
using Ticketera.BackgroundJobs;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Hangfire;
using System.IO;
using Volo.Abp.Content;
using QRCoder;
using System.Linq.Dynamic.Core;
using Volo.Abp.Application.Dtos;

namespace Ticketera.Events
{
    [Authorize]
    public class EventAppService : CrudAppService<Event, EventDto, long, EventResultRequestDto, EventInputDto>, IEventAppService
    {
        private readonly IRepository<EventDate, long> _eventDateRepo;
        private readonly IRepository<FileAttachment, long> _fileRepo;
        private readonly IRepository<EventProviderPayment, long> _eventProviderPaymentRepo;
        private readonly IStorageAppService _storageAppService;
        private readonly IRepository<Purchase, long> _purchaseRepo;
        private readonly IRepository<Ticket, long> _ticketRepo;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<UserEvent, long> _userEventRepo;
        private readonly IRepository<Volo.Abp.Identity.IdentityUser, Guid> _identityUserRepo;

        public EventAppService(IRepository<Event, long> repository, IRepository<EventDate, long> eventDateRepo, IRepository<FileAttachment, long> fileRepo, IStorageAppService storageAppService, IRepository<Purchase, long> purchaseRepo, IRepository<Ticket, long> ticketRepo, IHubContext<EventHub> hubContext, IRepository<EventProviderPayment, long> eventProviderPaymentRepo, IBackgroundJobManager backgroundJobManager, IConfiguration configuration, IEmailSender emailSender, IRepository<UserEvent, long> userEventRepo, IRepository<Volo.Abp.Identity.IdentityUser, Guid> identityUserRepo) : base(repository)
        {
            LocalizationResource = typeof(TicketeraResource);
            _eventDateRepo = eventDateRepo;
            _fileRepo = fileRepo;
            _storageAppService = storageAppService;
            _purchaseRepo = purchaseRepo;
            _ticketRepo = ticketRepo;
            _hubContext = hubContext;
            _eventProviderPaymentRepo = eventProviderPaymentRepo;
            _backgroundJobManager = backgroundJobManager;
            _configuration = configuration;
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];
            _emailSender = emailSender;
            _userEventRepo = userEventRepo;
            _identityUserRepo = identityUserRepo;
        }


        public override async Task<EventDto> CreateAsync(EventInputDto input)
        {
            input.CreatorId = CurrentUser.Id;
            input.Code = NormalizeEventName(input.Name);

            await ValidateEvent(input);

            var eve = await base.CreateAsync(input);
            await AssociateImagesWithEvent(eve.Id, input.PhotoGallery, input.PhotoDetail, input.PhotoLogo);
            foreach (var date in input.EventDatesInput)
            {
                var dateIn = ObjectMapper.Map<EventDateDto, EventDate>(date);
                dateIn.EventId = eve.Id;
                await _eventDateRepo.InsertAsync(dateIn);
            }
            var providerPayments = input.IdProviderPayment.Select(x =>
            {
                var eventProvider = new EventProviderPayment
                {
                    EventId = eve.Id,
                    ProviderPaymentId = x
                };
                return eventProvider;

            });
            await _eventProviderPaymentRepo.InsertManyAsync(providerPayments, true);

            // -- Create BackgroundJob for the days of the remaining and for sale event
            await UpdateEventDates(eve.Id);

            // -- Create BackgroungJob to dates
            await CreateOrEditBackgroundJobToEventForTickets(eve.Id);

            // -- Create relation whit event
            await CreateRelationWhitEvent(eve.Id, CurrentUser.Id.Value, TypeUserEvent.Admin);

            if (input.Validators.Count > 0)
            {
                foreach (var validator in input.Validators)
                {
                    await CreateRelationWhitEvent(eve.Id, validator.Key, validator.Value);
                }
            }

            return await GetAsync(eve.Id);
        }

        private async Task CreateRelationWhitEvent(long eventId, Guid userId, TypeUserEvent typeEvent)
        {
            var query = await Repository.WithDetailsAsync(x => x.UserEvents);
            var currentUserId = CurrentUser.Id.Value;

            var existEvent = query.Any(x => x.Id == eventId && (x.CreatorId == currentUserId || x.UserEvents.Any(y => y.IdentityUserId == currentUserId && (y.TypeUserEvent == TypeUserEvent.Admin || y.TypeUserEvent == TypeUserEvent.Editor))));

            if (!existEvent)
            {
                throw new UserFriendlyException(L["Error"]);
            }
            var userEvent = await _userEventRepo.FirstOrDefaultAsync(x => x.EventId == eventId && x.IdentityUserId == userId);

            if (userEvent == null)
            {
                userEvent = new UserEvent { IdentityUserId = userId, EventId = eventId, TypeUserEvent = typeEvent };
                await _userEventRepo.InsertAsync(userEvent, true);
            }
            else
            {
                userEvent.TypeUserEvent = typeEvent;
                await _userEventRepo.UpdateAsync(userEvent, true);
            }
        }

        private async Task AssociateImagesWithEvent(long eventId, long photoGalleryId, long photoDetailId, long photoLogoId)
        {
            var photoGallery = await _fileRepo.FirstOrDefaultAsync(x => x.Id == photoGalleryId);
            var photoDetail = await _fileRepo.FirstOrDefaultAsync(x => x.Id == photoDetailId);
            var photoLogo = await _fileRepo.FirstOrDefaultAsync(x => x.Id == photoLogoId);

            if (photoGallery != null)
            {
                photoGallery.EventId = eventId;
                photoGallery.IsDefault = true;
                await _fileRepo.UpdateAsync(photoGallery, true);
            }

            if (photoDetail != null)
            {
                photoDetail.EventId = eventId;
                photoDetail.FileType = FileAttachmentType.Detail;
                await _fileRepo.UpdateAsync(photoDetail, true);
            }

            if (photoLogo != null)
            {
                photoLogo.EventId = eventId;
                photoLogo.FileType = FileAttachmentType.Logo;
                await _fileRepo.UpdateAsync(photoLogo, true);
            }
        }

        private async Task DeleteImagesWithEvent(long id, EventInputDto input)
        {
            var query = await Repository.WithDetailsAsync(x => x.FileAttachments);
            var eve = query.FirstOrDefault(x => x.Id == id);

            if (input.PhotoDetail != 0 && eve != null)
            {
                var photoDetailFiles = eve.FileAttachments.Where(file => file.FileType == Enum.FileAttachmentType.Detail).ToList();

                foreach (var file in photoDetailFiles)
                {
                    await _storageAppService.RemoveFile(file.Id);
                }
            }

            if (input.PhotoGallery != 0 && eve != null)
            {
                var photoGalleryFiles = eve.FileAttachments.Where(file => file.FileType == Enum.FileAttachmentType.Gallery).ToList();

                foreach (var file in photoGalleryFiles)
                {
                    await _storageAppService.RemoveFile(file.Id); ;
                }
            }

            if (input.PhotoLogo != 0 && eve != null)
            {
                var photoLogoFiles = eve.FileAttachments.Where(file => file.FileType == Enum.FileAttachmentType.Logo).ToList();

                foreach (var file in photoLogoFiles)
                {
                    await _storageAppService.RemoveFile(file.Id);
                }
            }
        }

        private async Task ValidateEvent(EventInputDto input)
        {
            var eventExist = await ReadOnlyRepository.AnyAsync(x => (input.Id == 0 || (input.Id > 0 && x.Id != input.Id)) && x.Name.ToLower() == input.Name.ToLower());

            if (eventExist)
            {
                throw new UserFriendlyException(L["ExistEvent"]);
            }

            var codeExist = await ReadOnlyRepository.AnyAsync(x => (input.Id == 0 || (input.Id > 0 && x.Id != input.Id)) && x.Code.ToLower() == input.Code.ToLower());

            if (eventExist)
            {
                throw new UserFriendlyException(L["ExistCode"]);
            }
        }

        public override async Task<EventDto> UpdateAsync(long id, EventInputDto input)
        {
            // -- Validate event status
            if(input.EventStatus == EventStatus.Finalized)
            {
                throw new UserFriendlyException(L["EditEventFinalized"]);
            }

            // -- Validate user
            await ValidationProfile(id);

            input.Code = NormalizeEventName(input.Name);
            input.Id = id;
            await ValidateEvent(input);

            var eve = await Repository.FirstOrDefaultAsync(x => x.Id == id);

            if(eve == null)
            {
                throw new UserFriendlyException(L["NonExistentEvent"]);
            }

            input.CreatorId = eve.CreatorId;
            input.CreationTime = eve.CreationTime;
            var oldEventDates = await _eventDateRepo.GetListAsync(x => x.EventId == eve.Id);
            var newEventDatesSet = new HashSet<EventDateDto>(input.EventDatesInput);
            var eventDatesToRemove = oldEventDates.Where(x => !newEventDatesSet.Any(y => y.StartDate == x.StartDate && y.EndDate == x.EndDate)).ToList();

            foreach (var eventDate in eventDatesToRemove)
            {
                await _eventDateRepo.DeleteAsync(eventDate.Id, true);
            }

            foreach (var newEventDate in input.EventDatesInput)
            {
                if (!oldEventDates.Any(x => x.StartDate == newEventDate.StartDate && x.EndDate == newEventDate.EndDate))
                {
                    input.EventStatus = EventStatus.Available;
                    var eventDateToInsert = new EventDate
                    {
                        EventId = eve.Id,
                        StartDate = newEventDate.StartDate,
                        EndDate = newEventDate.EndDate
                    };
                    await _eventDateRepo.InsertAsync(eventDateToInsert, true);
                }
            }

            await _eventProviderPaymentRepo.DeleteAsync(x => x.EventId == eve.Id, true);

            var providerPayments = input.IdProviderPayment.Select(x =>
            {
                var eventProvider = new EventProviderPayment
                {
                    EventId = id,
                    ProviderPaymentId = x
                };
                return eventProvider;
            });

            await _eventProviderPaymentRepo.InsertManyAsync(providerPayments, true);
            await DeleteImagesWithEvent(id, input);
            await AssociateImagesWithEvent(id, input.PhotoGallery, input.PhotoDetail, input.PhotoLogo);
            var up = await base.UpdateAsync(id, input);

            // -- Create BackgroundJob for the days of the remaining and for sale event
            await UpdateEventDates(eve.Id);

            // -- Create BackgroungJob to dates
            await CreateOrEditBackgroundJobToEventForTickets(up.Id);

            var query = await _userEventRepo.GetQueryableAsync();
            var userEvents = query.Where(x => x.EventId == eve.Id).ToList();

            var userEventIdDelete = userEvents.Where(x => !input.Validators.Any(v => v.Key == x.IdentityUserId)).Select(x => x.Id).ToList();

            if (userEventIdDelete.Any())
            {
                await _userEventRepo.DeleteAsync(x => userEventIdDelete.Contains(x.Id), true);
            }

            var userEventIdInsert = input.Validators.Where(x =>
            {
                return !userEvents.Any(u => u.IdentityUserId == x.Key && u.TypeUserEvent == x.Value);
            }).ToDictionary(x => x.Key, x => x.Value);


            if (userEventIdInsert.Count > 0)
            {
                foreach (var validator in userEventIdInsert)
                {
                    await CreateRelationWhitEvent(eve.Id, validator.Key, validator.Value);
                }
            }

            await ShowUpdatedEventName(eve.Id);

            return up;
        }

        public override async Task DeleteAsync(long id)
        {
            // -- Validate user
            await ValidationProfile(id);

            var query = await Repository.WithDetailsAsync(x => x.EventDates, x => x.FileAttachments, x => x.EventProviders);
            var eve = query.FirstOrDefault(x => x.Id == id);

            if (eve != null)
            {
                foreach (var date in eve.EventDates)
                {
                    await _eventDateRepo.DeleteAsync(date.Id);
                }
                foreach (var file in eve.FileAttachments)
                {
                    await _storageAppService.RemoveFile(file.Id);
                }
                foreach (var provider in eve.EventProviders)
                {
                    await _eventProviderPaymentRepo.DeleteAsync(provider.Id);
                }
            }

            await base.DeleteAsync(id);
        }

        private async Task ValidationProfile(long id)
        {
            // -- Check if the user is authenticated
            var userId = CurrentUser.Id;
            if (userId == null)
                throw new UserFriendlyException(L["NoExistUserLogged"]);

            // -- Get the event
            var ev = (await ReadOnlyRepository
                .WithDetailsAsync(x => x.UserEvents))
                .FirstOrDefault(x => x.Id == id);

            // -- Check if the event exists and if the user has permission to access it
            if (ev == null || !HasPermission(ev, userId))
            {
                throw new UserFriendlyException(L["NotPermissionWithEvent"]);
            }
        }

        // -- Method to check if the user has permission to access the event
        private bool HasPermission(Event ev, Guid? userId)
        {
            // -- Check if the user is the creator of the event or has a specific role
            return ev.CreatorId == userId ||
                   ev.UserEvents.Any(x => x.IdentityUserId == userId &&
                                           (x.TypeUserEvent == TypeUserEvent.Admin ||
                                            x.TypeUserEvent == TypeUserEvent.Editor));
        }

        [AllowAnonymous]
        public override async Task<EventDto> GetAsync(long id)
        {
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.FileAttachments, x => x.EventDates, x => x.Tickets, x => x.EventProviders);

            var ev = query.First(x => x.Id == id);

            var queryUserEvent = await _userEventRepo.WithDetailsAsync(x => x.IdentityUser);
            var userEvents = queryUserEvent.Where(x => x.EventId == id).Select(x => ObjectMapper.Map<UserEvent, UserEventDto>(x));

            var evDto = ObjectMapper.Map<Event, EventDto>(ev);
            evDto.UserEvents = userEvents;

            return evDto;
        }

        [AllowAnonymous]
        public async Task<EventDto> GetByCode(string code)
        {
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.FileAttachments, x => x.EventDates);

            var ev = query.First(x => x.Code == code);

            var evDto = ObjectMapper.Map<Event, EventDto>(ev);
            evDto.PhotoGallery = AddImageSizeParameters(evDto.PhotoGallery, nameof(EventDto.PhotoGallery), false);
            evDto.PhotoDetail = AddImageSizeParameters(evDto.PhotoDetail, nameof(EventDto.PhotoDetail), false);
            evDto.PhotoLogo = AddImageSizeParameters(evDto.PhotoLogo, nameof(EventDto.PhotoLogo), false);
            
            return evDto;
        }

        protected override IQueryable<Event> ApplySorting(IQueryable<Event> query, EventResultRequestDto input)
        {
            IQueryable<Event> queryOrder;

            if (!string.IsNullOrEmpty(input.Order) && input.Order.ToLower() == "desc")
            {
                queryOrder = query.OrderByDescending(x => x.EventStatus == EventStatus.Available || x.EventStatus == EventStatus.SoldOut ? 0 :
                                                          x.EventStatus == EventStatus.Finalized ? 2 : 1)
                                  .ThenByDescending(x => x.EventStatus == EventStatus.Available || x.EventStatus == EventStatus.SoldOut ? x.EventDates.Min(o => o.StartDate) : DateTime.MaxValue)
                                  .ThenBy(x => x.EventStatus == EventStatus.Finalized ? x.EventDates.Min(o => o.StartDate) : DateTime.MaxValue);
            }
            else
            {
                queryOrder = query.OrderBy(x => x.EventStatus == EventStatus.Available || x.EventStatus == EventStatus.SoldOut ? 0 :
                                                 x.EventStatus == EventStatus.Finalized ? 2 : 1)
                                  .ThenBy(x => x.EventStatus == EventStatus.Available || x.EventStatus == EventStatus.SoldOut ? x.EventDates.Min(o => o.StartDate) : DateTime.MaxValue)
                                  .ThenByDescending(x => x.EventStatus == EventStatus.Finalized ? x.EventDates.Min(o => o.StartDate) : DateTime.MinValue);
            }

            return queryOrder;
        }

        protected override async Task<IQueryable<Event>> CreateFilteredQueryAsync(EventResultRequestDto input)
        {
            var userId = input.UserId ?? CurrentUser.Id;
            //var query = await base.CreateFilteredQueryAsync(input);

            // --Events with EventDates and Files
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.EventDates, x => x.FileAttachments, x => x.UserEvents);

            // -- Add filter to user
            query = query.Where(x =>
                        (!input.IsOwner || (input.IsOwner && userId.HasValue && (x.CreatorId == null || (x.CreatorId != null && x.CreatorId == userId.Value)))) ||
                        (!input.IsAdmin || (input.IsAdmin && userId.HasValue && (x.UserEvents.Any(ue => ue.IdentityUserId == userId.Value && ue.TypeUserEvent == TypeUserEvent.Admin)))) ||
                        (!input.IsEditor || (input.IsEditor && userId.HasValue && (x.UserEvents.Any(ue => ue.IdentityUserId == userId.Value && ue.TypeUserEvent == TypeUserEvent.Editor)))) ||
                        (!input.IsValidator || (input.IsValidator && userId.HasValue && (x.UserEvents.Any(ue => ue.IdentityUserId == userId.Value && ue.TypeUserEvent == TypeUserEvent.Validator))))
            );

            // -- Add filters
            query = query.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => (x.Name + " " + x.Description + " " + x.Place).ToLower().Contains(input.Keyword.ToLower()));

            // -- Filter by DateFrom and DateTo
            // -- Set date now            
            if (input.DateFrom.HasValue)
            {
                input.DateFrom = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                query = query.Where(x => x.EventDates.Any(d => d.StartDate >= input.DateFrom));
            }

            if (input.DateTo.HasValue)
            {
                input.DateTo = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
            }
            query = query.WhereIf(input.DateTo.HasValue, x => x.EventDates.Any(ed => ed.StartDate.Date <= input.DateTo));

            // -- Filter to type
            query = query.WhereIf(input.EventType.HasValue, x => x.EventType == input.EventType.Value);

            // -- To Carrousel
            query = query.WhereIf(input.IsMain.HasValue && input.IsMain.Value,
                      x => x.IsMain == input.IsMain.Value && x.FileAttachments.Any(f => f.IsDefault) && x.EventStatus != EventStatus.Finalized);

            // -- Eclude id to event
            query = query.WhereIf(input.ExcludeIds.Any(), x => !input.ExcludeIds.Contains(x.Id));

            return query;
        }

        [AllowAnonymous]
        public override async Task<PagedResultDto<EventDto>> GetListAsync(EventResultRequestDto input)
        {
            var list = await base.GetListAsync(input);
            foreach (var eventDto in list.Items)
            {
                eventDto.PhotoGallery = AddImageSizeParameters(eventDto.PhotoGallery, nameof(EventDto.PhotoGallery), input.IsMobile);
                eventDto.PhotoDetail = AddImageSizeParameters(eventDto.PhotoDetail, nameof(EventDto.PhotoDetail), input.IsMobile);
                eventDto.PhotoLogo = AddImageSizeParameters(eventDto.PhotoLogo, nameof(EventDto.PhotoLogo), input.IsMobile);
            }

            return list;
        }

        protected override async Task<EventDto> MapToGetOutputDtoAsync(Event entity)
        {
            var userId = CurrentUser.Id;
            var map = await base.MapToGetOutputDtoAsync(entity);

            if (userId.HasValue)
            {
                // -- Is Owner
                map.IsOwner = entity.CreatorId != null && entity.CreatorId == userId;

                // -- Is Admin
                map.IsAdmin = map.IsOwner || entity.UserEvents.Any(ue => ue.IdentityUserId == userId.Value && ue.TypeUserEvent == TypeUserEvent.Admin);

                // -- Is Editor
                map.IsEditor = map.IsOwner || entity.UserEvents.Any(ue => ue.IdentityUserId == userId.Value && ue.TypeUserEvent == TypeUserEvent.Editor);

                // -- Is Validator
                map.IsValidator = map.IsOwner || entity.UserEvents.Any(ue => ue.IdentityUserId == userId.Value && ue.TypeUserEvent == TypeUserEvent.Validator);

                return map;
            }

            return map;
        }

        [AllowAnonymous]
        public async Task<string> PurchaseCreate()
        {
            // -- Create Purchase
            var purchase = new Purchase();
            purchase = await _purchaseRepo.InsertAsync(purchase, true);

            // -- Remove if expired time
            if (_backgroundJobManager.IsAvailable())
            {
                // -- Create backgroundJob to delete purchase
                await _backgroundJobManager.EnqueueAsync(new CancelPurchaseBackgroundJobArgs
                {
                    Code = purchase.Code
                }, delay: TimeSpan.FromMinutes(10));
            }

            return purchase.Code.ToString();
        }

        [AllowAnonymous]
        public async Task PurchaseCancel(string code)
        {
            // --Remove if expired time
            if (_backgroundJobManager.IsAvailable())
            {
                // -- Create backgroundJob to delete purchase
                await _backgroundJobManager.EnqueueAsync(new CancelPurchaseBackgroundJobArgs
                {
                    Code = code
                });
            }
        }


        [AllowAnonymous]
        public async Task PuchaseCreateOrUpdateTicket(PurchaseTicketInputDto input)
        {
            // -- Get event
            var ev = await Repository.FirstOrDefaultAsync(x => x.Id == input.EventId);

            // -- Check if event exists
            if (ev == null)
            {
                // Handle case where event does not exist
                throw new UserFriendlyException("Event not found.");
            }

            // -- Check if event active
            if (!ev.IsActive)
            {
                throw new UserFriendlyException(L["PausedEvent"]);
            }

            // -- Check if any event date is in the past
            if (ev.EventDates.Any(ed => ed.StartDate < DateTime.Now))
            {
                // Handle case where event date is in the past
                throw new UserFriendlyException("Cannot purchase tickets for past events.");
            }

            var query = await _purchaseRepo.WithDetailsAsync(x => x.Ticktes);
            var purchaseTemp = query.First(x => x.Code == input.Code && x.State == PurchaseState.Pending) ?? throw new UserFriendlyException(L["ExpiredReserveTicket"]);

            // -- Validate if exist tickets
            await ValidateTicket(input);

            // -- Check count ticket
            var countExist = purchaseTemp.Ticktes.Count(x => x.TicketCategoryId == input.TicketCategoryId && x.TicketSectorId == input.TicketSectorId);

            // -- Remove tickets
            if (countExist > input.Count)
            {
                var ticketDelete = purchaseTemp.Ticktes.Where(x => x.TicketCategoryId == input.TicketCategoryId && x.TicketSectorId == input.TicketSectorId).Skip(input.Count).Select(x => x.Id).ToList();
                await _ticketRepo.DeleteManyAsync(ticketDelete, true);
            }

            // -- Insert tickets
            if (input.Count > countExist)
            {
                // -- Check to insert
                var ticketCount = input.Count - countExist;
                var ticketInsert = new List<Ticket>();
                for (var i = 0; i < ticketCount; i++)
                {
                    // -- Get price
                    var price = ev.Prices.First(x => x.TicketCategoryId == input.TicketCategoryId && x.TicketSectorId == input.TicketSectorId).Price;

                    // -- Create map to ticket
                    var tic = ObjectMapper.Map<PurchaseTicketInputDto, Ticket>(input);
                    tic.Title = $"Entrada para: {ev.Name.ToUpper()}";
                    tic.EventId = input.EventId;
                    tic.EventDateId = input.EventDateId;
                    tic.Price = price;
                    tic.PurchaseId = purchaseTemp.Id;
                    tic.Code = string.Empty;

                    // -- Add to list ticket
                    ticketInsert.Add(tic);
                }

                // -- Insert masive tickets
                await _ticketRepo.InsertManyAsync(ticketInsert, true);
            }


            // -- Notificatión when new purchase reserved            
            await _hubContext.Clients.All.SendAsync(TicketeraConsts.NotificationTicket, input.EventId);
        }

        [AllowAnonymous]
        public async Task<IList<PurchaseTicketDto>> GetTicketAvailable(TicketAvailableDto input)
        {
            var eventId = input.EventId;
            var eventDateId = input.EventDateId;

            // -- Get event ids
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.EventDates);

            // -- Get event
            var ev = query.First(x => x.Id == eventId);

            // -- Get price and count total tickets
            var prices = ev.Prices;

            // -- Get list tickets available
            var response = new List<PurchaseTicketDto>();

            foreach (var price in prices.OrderBy(x => x.Price))
            {
                var ticketCount = await _ticketRepo.CountAsync(x =>
                    x.EventId == eventId &&
                    x.EventDateId == eventDateId &&
                    x.TicketCategoryId == price.TicketCategoryId &&
                    x.TicketSectorId == price.TicketSectorId &&
                    (x.TicketStatus == TicketStatus.Sold || x.TicketStatus == TicketStatus.Reserved));

                var purchaseTicket = new PurchaseTicketDto
                {
                    Price = price.Price,
                    Count = price.Count - ticketCount,
                    TicketCategoryId = price.TicketCategoryId,
                    TicketCategoryName = price.TicketCategoryName,
                    TicketSectorId = price.TicketSectorId,
                    TicketSectorName = price.TicketSectorName,
                };

                response.Add(purchaseTicket);
            }

            var listResponse = response.ToList();

            return listResponse;
        }

        private async Task ValidateTicket(PurchaseTicketInputDto input)
        {
            // -- Get event ids
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.EventDates);

            // -- Get event
            var ev = query.First(x => x.Id == input.EventId);

            // -- Get date
            var date = ev.EventDates.First(x => x.Id == input.EventDateId);

            // -- Get available tickets
            var availableTickets = await GetTicketAvailable(new TicketAvailableDto
            {
                EventDateId = input.EventDateId,
                EventId = input.EventId
            });

            // -- Get available ticket
            var availableTicket = availableTickets.First(x => x.TicketCategoryId == input.TicketCategoryId && x.TicketSectorId == input.TicketSectorId);

            // -- Get price and count total tickets
            var price = ev.Prices.First(x => x.TicketSectorId == input.TicketSectorId && x.TicketCategoryId == input.TicketCategoryId);

            // -- Check if exist ticket 
            var noExistTicketPerDay = (availableTicket.Count - 1) < 0;
            if (noExistTicketPerDay)
            {
                throw new UserFriendlyException(L["NoExistTicketPerDay", price.TicketSectorName, price.TicketCategoryName, date.StartDate.ToString("dd.MM.yy HH:mm")]);
            }
        }

        [AllowAnonymous]
        public async Task<string> PurchaseProcess(PurchaseInputDto input, decimal inputCommission, int seconds)
        {
            // -- Get event
            var queryEve = await Repository.WithDetailsAsync(x => x.EventDates);
            var ev = queryEve.First(x => x.Id == input.EventId);

            // -- Get date
            var date = await _eventDateRepo.FirstOrDefaultAsync(x => x.Id == input.EventDateId);

            // -- Check if event exists
            if (ev == null)
            {
                // Handle case where event does not exist
                throw new UserFriendlyException("Event not found.");
            }

            // -- Check if eventDate exists
            if (date == null)
            {
                // Handle case where event does not exist
                throw new UserFriendlyException(L["DateNotFound"]);
            }

            // -- Check if event active
            if (!ev.IsActive)
            {
                throw new UserFriendlyException(L["PurchaseDisableb"]);
            }

            // -- Check if the last event date is in the past
            if (ev.EventDates.OrderBy(ed => ed.StartDate).Last().StartDate < DateTime.Now)
            {
                // Handle case where the last event date is in the past
                throw new UserFriendlyException("Cannot purchase tickets for past events.");
            }

            var now = DateTime.Now;

            // Determine if the event is within the next time periods
            var isEventWithin24Hours = date.StartDate < now.AddHours(24);
           
            // TODO: Get commission from config tenant
            var commission = inputCommission;

            // Round commission to two decimal places
            commission = Convert.ToDecimal(commission.ToString("F2"));

            // -- Get purchase
            var query = await _purchaseRepo.WithDetailsAsync(x => x.Ticktes);
            var purchase = query.First(x => x.Code == input.Code);

            // -- Urls backs
            var urlFrontend = $"{_configuration["App:ClientUrl"]}/event/{ev.Code}/response";
            var urlBackend = $"{_configuration["App:SelfUrl"]}/api/app/event/purchase-response";

#if DEBUG
            urlBackend = $"https://312f-132-255-225-169.ngrok-free.app/api/app/event/purchase-response";
#endif

            // -- Create object to request preference
            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
                {
                new PreferenceItemRequest
                {
                    Title = $"Compra de entradas para: {ev.Name.ToUpper()}",
                    Quantity = 1,
                    CurrencyId = "MXN",
                    UnitPrice = purchase.Ticktes.Sum(x => x.Price) + commission,
                },
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = urlFrontend,
                    Failure = urlFrontend,
                    Pending = urlFrontend
                },
                ExternalReference = purchase.Code.ToString(),
                Payer = new PreferencePayerRequest
                {
                    Name = input.Name,
                    Surname = input.Surname,
                    Email = input.Email,
                    Phone = new PhoneRequest
                    {
                        AreaCode = "+54",
                        Number = input.Phone
                    }
                },
                AutoReturn = "approved",
                NotificationUrl = urlBackend,
                PaymentMethods = isEventWithin24Hours ? new PreferencePaymentMethodsRequest
                {
                    ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                    {
                        new PreferencePaymentTypeRequest { Id = "ticket" },
                        new PreferencePaymentTypeRequest { Id = "atm" }
                    }
                } : null,
                Expires = true,
                ExpirationDateTo = now.AddSeconds(seconds),
                DateOfExpiration = now.AddDays(3)
            };

            // -- Create product preference with client
            var client = new PreferenceClient();
            var preference = await client.CreateAsync(request);

            // -- Save purchase and preference
            purchase = ObjectMapper.Map(input, purchase);
            purchase.State = PurchaseState.InProcess;
            purchase.MercadoPagoPreference = JsonConvert.SerializeObject(preference);
            await _purchaseRepo.UpdateAsync(purchase, true);

            // -- Remove if expired time
            if (_backgroundJobManager.IsAvailable())
            {
                // -- Create backgroundJob to delete purchase                        
                await _backgroundJobManager.EnqueueAsync(new CancelPurchaseBackgroundJobArgs
                {
                    Code = purchase.Code,
                    State = PurchaseState.InProcess,
                }, delay: TimeSpan.FromSeconds(seconds + 5));
            }
            
            // -- Return url to redirect for Environment Mercado Pago
            return preference.InitPoint;
        }

        [AllowAnonymous]
        public async Task<bool> ValidateTicketPurchaseAsync(long eventId, int quantity)
        {
            var eventEntity = await Repository.GetAsync(eventId);

            int saleForPerson = eventEntity.SaleForPerson;

            if (saleForPerson > 0 && (quantity + 1) > saleForPerson)
            {
                return false;
            }

            return true;
        }


        [AllowAnonymous]
        public async Task<string?> PurchaseCheck(PurchaseCheckDto input)
        {
            // -- Get purchase check
            var code = input.ExternalReference;

            // -- Get purchase
            var purchase = await _purchaseRepo.FirstAsync(x => x.Code == code);

            // -- Get preference product
            var preference = JsonConvert.DeserializeObject<Preference>(purchase.MercadoPagoPreference);

            // -- Create Payment Client
            var client = new PaymentClient();
            var response = await client.GetAsync(long.Parse(input.PaymentId));

            // -- Check if is preference
            if (preference?.CollectorId == response.CollectorId)
            {
                // -- Return approved or pending or failure
                return response.Status;
            }

            return null;
        }

        [AllowAnonymous]
        public async Task PurchaseResponse(PurchaseResponseJsonDto input)
        {
            if (!input.Type.IsNullOrEmpty() && input.Data != null && input.Type == "payment")
            {
                var paymentId = input.Data.Id;

                // -- Create Payment Client
                var client = new PaymentClient();
                var response = await client.GetAsync(long.Parse(paymentId));

                // -- If status is approved
                if (response != null && response.Status == "approved")
                {
                    var purchase = await _purchaseRepo.FirstAsync(x => x.Code == response.ExternalReference);
                    purchase.State = PurchaseState.Finish;

                    purchase.MercadoPagoResponse = JsonConvert.SerializeObject(response);

                    var query = await _ticketRepo.WithDetailsAsync(x => x.EventDate);
                    var tickets = query.Where(x => x.PurchaseId == purchase.Id).ToList()
                                  .Select(x =>
                                  {
                                      x.TicketStatus = TicketStatus.Sold;
                                      return x;
                                  }).ToList();

                    await _ticketRepo.UpdateManyAsync(tickets, true);
                    await _purchaseRepo.UpdateAsync(purchase, true);

                    // -- Notificatión when new purchase reserved            
                    await _hubContext.Clients.All.SendAsync(TicketeraConsts.NotificationTicketSold, purchase.Code);

                    // -- Send email to approved purchase
                    await SendPurchaseConfirmationEmail(purchase, tickets);

                    // -- Background task
                    await QrEmailSendingEnabled(tickets);

                    // -- Validate event status
                    await IsEventSoldOut(tickets[0].EventId);

                    // -- Events statistic
                    await GetDashboardStatisticsAsync(tickets[0].EventId);

                    // -- Tickets statistics
                    await GetTicketsSoldAndAvailable(tickets[0].EventId);

                    // -- Tickets statistics for type
                    await GetTicketsSoldAndAvailableForType(tickets[0].EventId);

                    // -- Total tickets statistics
                    await GetTicketsStatistics(tickets[0].EventId);
                }

                // -- If status is distint approved or pending
                if (response != null && response.Status != "approved" && response.Status != "pending" && response.Status != "in_process")
                {
                    var purchase = await _purchaseRepo.FirstAsync(x => x.Code == response.ExternalReference);

                    await PurchaseCancel(purchase.Code);

                    // -- Notificatión when new purchase reserved            
                    await _hubContext.Clients.All.SendAsync(TicketeraConsts.NotificationTicketSold, purchase.Code);

                    // -- Send email to purchase error
                    await SendPurchaseErrorEmail(purchase);
                }

                // -- If status is pending
                if (response != null && (response.Status == "pending" || response.Status == "in_process"))
                {
                    var purchase = await _purchaseRepo.FirstAsync(x => x.Code == response.ExternalReference);
                    purchase.State = PurchaseState.Pending;

                    await _purchaseRepo.UpdateAsync(purchase, true);

                    // -- Send email to purchase pending
                    await SendPurchasePendingEmail(purchase);
                }
            }
        }

        private async Task CreateOrEditBackgroundJobToEventForTickets(long eventId)
        {
            var eventDates = await _eventDateRepo.GetListAsync(x => x.EventId == eventId);

            // -- Delete BackgroundJob
            eventDates.Where(x => !string.IsNullOrEmpty(x.BackgroundJobId))
                      .Select(async eventDate =>
                      {
                          BackgroundJob.Delete(eventDate.BackgroundJobId);
                      });


            // -- Create BackgroundJob
            var dateNow = DateTime.Now;
            var dates = await eventDates.OrderBy(x => x.StartDate).Select(async eventDate =>
            {
                var dateInit = eventDate.StartDate.AddHours(-10);
                var delay = dateInit - dateNow;

                var jobId = await _backgroundJobManager.EnqueueAsync(new EventRefreshTicketBackgroundJobArgs
                {
                    EventDateId = eventDate.Id,
                }, delay: delay);

                eventDate.BackgroundJobId = jobId;

                return eventDate;
            }).WhenAll();

            await _eventDateRepo.UpdateManyAsync(dates, true);
        }

        private async Task QrEmailSendingEnabled(List<Ticket> tickets)
        {
            var date = DateTime.Now;
            var eventDate = await _eventDateRepo.FirstOrDefaultAsync(d => d.Id == tickets[0].EventDateId);

            if (eventDate != null)
            {
                var delay = TimeSpan.FromSeconds(0);

                if(date < eventDate.StartDate.AddHours(-10))
                {
                    delay = eventDate.StartDate.AddHours(-10) - date;
                    await _backgroundJobManager.EnqueueAsync(new QrEmailEnabledBackgroundJobArgs
                    {
                        EventId = eventDate.EventId,
                        PurchaseId = tickets[0].PurchaseId,
                        EventDateId = eventDate.Id,
                    }, delay: delay);
                }
            }
        }

        [AllowAnonymous]
        public async Task<PurchaseDto> PurchaseGet(string code)
        {
            var purchase = await _purchaseRepo.FirstAsync(x => x.Code == code);

            var response = ObjectMapper.Map<Purchase, PurchaseDto>(purchase);

            return response;
        }

        private static string NormalizeEventName(string eventName)
        {
            eventName = Regex.Replace(eventName, @"[\s""&%$\/()=?¿!'<>{}¡@,]+|[\[\]]", "-");
            eventName = RemoveAccentedLetters(eventName);
            eventName = eventName.Trim('-');

            return eventName.ToLower();
        }

        private static string RemoveAccentedLetters(string text)
        {
            text = Regex.Replace(text, "[áÁ]", "a");
            text = Regex.Replace(text, "[éÉ]", "e");
            text = Regex.Replace(text, "[íÍ]", "i");
            text = Regex.Replace(text, "[óÓ]", "o");
            text = Regex.Replace(text, "[úÚ]", "u");
            text = Regex.Replace(text, "[ñÑ]", "n");

            return text;
        }

        private async Task SendPurchaseConfirmationEmail(Purchase purchase, List<Ticket> tickets)
        {
            try
            {
                var mercadoPagoResponseObject = JsonConvert.DeserializeObject<MercadoPago.Resource.Payment.Payment>(purchase.MercadoPagoResponse);
                decimal? transactionAmount = mercadoPagoResponseObject?.TransactionAmount;
                if (transactionAmount.HasValue)
                {
                    transactionAmount = Math.Round(transactionAmount.Value, 2, MidpointRounding.AwayFromZero);
                }

                var eventId = purchase.Ticktes[0].EventId;
                var dateId = purchase.Ticktes[0].EventDateId;
                var query = await Repository.WithDetailsAsync(x => x.EventDates);
                var eve = query.FirstOrDefault(x => x.Id == eventId);
                var eventDate = query.SelectMany(e => e.EventDates)
                    .FirstOrDefault(d => d.Id == dateId && d.EventId == eventId);

                CultureHelper.Use("es-ES");

                string listTickets = Resource.list_tickets;
                string ticketRows = "";
                foreach (var ticket in tickets)
                {
                    var ticketCategoryId = ticket.TicketCategoryId;
                    var ticketCategory = eve.Prices.FirstOrDefault(p => p.TicketCategoryId == ticketCategoryId);

                    var ticketSectorId = ticket.TicketSectorId;
                    var ticketSector = eve.Prices.FirstOrDefault(p => p.TicketSectorId == ticketSectorId);
                    var urlTicket = $"{_configuration["App:ClientUrl"]}/event/{eve.Code}/purchase/{purchase.Code}/{ticket.Id}";
                    string row = listTickets
                        .Replace("{{infoTicket}}", "1 x " + ticketCategory.TicketCategoryName + " - " + ticketSector.TicketSectorName)
                        .Replace("{{text11}}", L["Text11"])
                        .Replace("{{urlTicket}}", urlTicket);
                    ticketRows += row;
                }

                string purchaseConfirmationHtml = Resource.purchase_confirmation;
                string body = purchaseConfirmationHtml
                    .Replace("{{text1}}", L["Text1"])
                    .Replace("{{text2}}", L["Text2"] + purchase.Name)
                    .Replace("{{text3}}", L["Text3"])
                    .Replace("{{nameEvent}}", eve.Name)
                    .Replace("{{list}}", ticketRows)
                    .Replace("{{text4}}", L["Text4"])
                    .Replace("{{fullPayment}}", "$ " + (transactionAmount.HasValue ? transactionAmount.Value.ToString("F2") : "0.00"))
                    .Replace("{{text5}}", L["Place"])
                    .Replace("{{place}}", eve.Place)
                    .Replace("{{text6}}", L["Text6"])
                    .Replace("{{dateEvent}}", eventDate.StartDate.ToString("dd/MM/yyyy"))
                    .Replace("{{text7}}", L["Text7"])
                    .Replace("{hourEvent}", eventDate.StartDate.ToString("HH:mm"))
                    .Replace("{{text8}}", L["Text8"])
                    .Replace("{{text9}}", L["Text9"])
                    .Replace("{{text10}}", L["Text10"])
                    .Replace("{{text12}}", L["Text12"] + purchase.Id)
                    .Replace("{{logoTixGo}}", $"{_configuration["App:SelfUrl"]}/resources/LogoTixGo.png")
                    .Replace("{{qrDisabled}}", $"{_configuration["App:SelfUrl"]}/resources/QrDisabled.png")
                    .Replace("{{facebookIcon}}", $"{_configuration["App:SelfUrl"]}/resources/FacebookIcon.png")
                    .Replace("{{instagramIcon}}", $"{_configuration["App:SelfUrl"]}/resources/InstagramIcon.png")
                    .Replace("{{twitterIcon}}", $"{_configuration["App:SelfUrl"]}/resources/TwitterIcon.png");

                var subject = L["PurchaseConfirmation"];

                await _emailSender.SendAsync(
                    purchase.Email,
                    subject,
                    body,
                    true
                );
            }
            catch (Exception)
            {
                throw new UserFriendlyException(L["ErrorEmail"]);
            }
        }

        private async Task SendPurchaseErrorEmail(Purchase purchase)
        {
            CultureHelper.Use("es-ES");

            try
            {
                string purchaseConfirmationHtml = Resource.purchase_error;

                string body = purchaseConfirmationHtml
                    .Replace("{{paymentError}}", L["PaymentError"])
                    .Replace("{{descriptionPaymentError}}", L["DescriptionPaymentError"])
                    .Replace("{{text9}}", L["Text9"])
                    .Replace("{{text10}}", L["Text10"])
                    .Replace("{{logoTixGo}}", $"{_configuration["App:SelfUrl"]}/resources/LogoTixGo.png")
                    .Replace("{{imageSadFace}}", $"{_configuration["App:SelfUrl"]}/resources/SadFace.png")
                    .Replace("{{facebookIcon}}", $"{_configuration["App:SelfUrl"]}/resources/FacebookIcon.png")
                    .Replace("{{instagramIcon}}", $"{_configuration["App:SelfUrl"]}/resources/InstagramIcon.png")
                    .Replace("{{twitterIcon}}", $"{_configuration["App:SelfUrl"]}/resources/TwitterIcon.png");

                var subject = L["PaymentError"];

                await _emailSender.SendAsync(
                    purchase.Email,
                    subject,
                    body,
                    true
                );
            }
            catch (Exception)
            {
                throw new UserFriendlyException(L["ErrorEmail"]);
            }
        }
        private async Task SendPurchasePendingEmail(Purchase purchase)
        {
            CultureHelper.Use("es-ES");

            try
            {
                string purchaseConfirmationHtml = Resource.purchase_pending;

                string body = purchaseConfirmationHtml
                    .Replace("{{text2}}", L["Text2"] + purchase.Name)
                    .Replace("{{text3}}", L["Text3"])
                    .Replace("{{paymentPending}}", L["PaymentPending"])
                    .Replace("{{descriptionPaymentPending}}", L["DescriptionPaymentPending"])
                    .Replace("{{text9}}", L["Text9"])
                    .Replace("{{text10}}", L["Text10"])
                    .Replace("{{logoTixGo}}", $"{_configuration["App:SelfUrl"]}/resources/LogoTixGo.png")
                    .Replace("{{imagePending}}", $"{_configuration["App:SelfUrl"]}/resources/Clock.png")
                    .Replace("{{facebookIcon}}", $"{_configuration["App:SelfUrl"]}/resources/FacebookIcon.png")
                    .Replace("{{instagramIcon}}", $"{_configuration["App:SelfUrl"]}/resources/InstagramIcon.png")
                    .Replace("{{twitterIcon}}", $"{_configuration["App:SelfUrl"]}/resources/TwitterIcon.png");

                var subject = L["PaymentPending"];

                await _emailSender.SendAsync(
                    purchase.Email,
                    subject,
                    body,
                    true
                );
            }
            catch (Exception)
            {
                throw new UserFriendlyException(L["ErrorEmail"]);
            }
        }

        [AllowAnonymous]
        public async Task<bool> ValidateCode(string code)
        {
            return await Repository.AnyAsync(x => x.Code.ToLower() == code.ToLower());
        }

        [AllowAnonymous]
        public async Task<IRemoteStreamContent?> GetTicketCode(TicketInputDto input)
        {
            var exist = await _ticketRepo.FirstOrDefaultAsync(x => x.Id == input.TicketId && x.Purchase.Code == input.PurchaseCode && x.Code != "");

            if (exist != null)
            {
                if (exist.TicketStatus == TicketStatus.SoldUsed)
                {
                    string pathEnvironmentSoldUsed = AppDomain.CurrentDomain.BaseDirectory;
                    string imagePathSoldUsed = Path.Combine(pathEnvironmentSoldUsed, "Resources", "QrValidate.png");

                    byte[] fileBytesSoldUsed = GetFileBytes(imagePathSoldUsed);
                    using var memoryStreamQrSoldUsed = new MemoryStream(fileBytesSoldUsed);
                    return new RemoteStreamContent(memoryStreamQrSoldUsed, "Ticket Usado");
                }

                if (exist.TicketStatus == TicketStatus.Sold)
                {

                    var qrGenerator = new QRCodeGenerator();
                    var qrCodeData = qrGenerator.CreateQrCode(exist.Code, QRCodeGenerator.ECCLevel.Q);
                    var qrCode = new PngByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);

                    using var memoryStream = new MemoryStream(qrCodeImage);
                    return new RemoteStreamContent(memoryStream, exist.Code);
                }
            }

            string basePath = Directory.GetCurrentDirectory();
            string pathEnvironment = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.Combine(pathEnvironment, "Resources", "QrDisabled.png");

            byte[] fileBytes = GetFileBytes(imagePath);
            using var memoryStreamQr = new MemoryStream(fileBytes);
            return new RemoteStreamContent(memoryStreamQr, "NotFound");
        }

        private byte[] GetFileBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("El archivo no existe", filePath);
            }

            return File.ReadAllBytes(filePath);
        }

        [AllowAnonymous]
        public async Task<TicketOutDto> GetTicket(TicketInputDto input)
        {
            var query = await _ticketRepo.WithDetailsAsync(x => x.EventDate, x => x.TicketCategory, x => x.TicketSector, x => x.Purchase, x => x.Event);
            var ticket = query.First(x => x.Id == input.TicketId && x.Purchase.Code == input.PurchaseCode);

            var ticketResult = ObjectMapper.Map<Ticket, TicketOutDto>(ticket);

            ticketResult.SoldUsed = ticket.TicketStatus == TicketStatus.SoldUsed;

            return ticketResult;
        }

        public async Task<TicketValidateOutDto> TicketValidate(TicketValidateInputDto input)
        {
            var userId = CurrentUser.Id;

            var queryEvent = await ReadOnlyRepository.WithDetailsAsync(x => x.UserEvents);
            var existEventWithUserWithValidator = queryEvent.FirstOrDefault(x => x.Id == input.EventId && (x.CreatorId == userId || x.UserEvents.Any(ue => ue.IdentityUserId == userId && (ue.TypeUserEvent == TypeUserEvent.Admin || ue.TypeUserEvent == TypeUserEvent.Validator))));

            if (existEventWithUserWithValidator == null)
            {
                return new TicketValidateOutDto
                {
                    Status = TicketValidateType.Fail
                };
            }

            var queryTicket = await _ticketRepo.WithDetailsAsync(x => x.Purchase, x => x.EventDate, x => x.TicketCategory, x => x.TicketSector);
            var ticket = queryTicket.FirstOrDefault(x => x.EventId == input.EventId && x.Code == input.TicketCode.Trim());

            if (ticket != null)
            {
                if (ticket.TicketStatus == TicketStatus.SoldUsed)
                {
                    return new TicketValidateOutDto
                    {
                        Status = TicketValidateType.Already
                    };
                }

                if (ticket.TicketStatus == TicketStatus.Sold)
                {
                    ticket.TicketStatus = TicketStatus.SoldUsed;
                    await _ticketRepo.UpdateAsync(ticket, true);
                    await _hubContext.Clients.All.SendAsync(existEventWithUserWithValidator.Code + "-" + ticket.EventDate.StartDate.ToString("yyMMddHHmm"), true);
                    await _backgroundJobManager.EnqueueAsync(new SendingValidatedTicketByEmailBackgroundJobArgs
                    {
                        TicketId = ticket.Id
                    });

                    return new TicketValidateOutDto
                    {
                        ClientName = $"{ticket.Purchase.Name} {ticket.Purchase.Surname}",
                        CategoryName = ticket.TicketCategory.Name,
                        SectorName = ticket.TicketSector.Name,
                        Status = TicketValidateType.Success
                    };
                }
            }

            return new TicketValidateOutDto
            {
                Status = TicketValidateType.Fail
            };
        }

        public async Task<IList<UserOutDto>> GetUsersEvent(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<UserOutDto>();
            }

            var query = await _identityUserRepo.GetQueryableAsync();
            var keyword = input.Trim().ToLower();

            // -- Add filters
            var filteredQuery = query
                     .WhereIf(!string.IsNullOrEmpty(keyword), x => x.Name.ToLower().Contains(keyword) ||
                                                              x.Surname.ToLower().Contains(keyword) ||
                                                              x.Email.ToLower().Contains(keyword))
                                                              .Take(5);

            var userList = filteredQuery.ToList();

            var userListDto = userList.Select(x => new UserOutDto
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Email = x.Email
            }).ToList();

            return userListDto;
        }

        public async Task UpdateEventStatus(long id, bool isActive)
        {
            // -- Validate user
            await ValidationProfile(id);
            var ev = await Repository.FirstOrDefaultAsync(x => x.Id == id);

            if (ev == null)
            {
                throw new UserFriendlyException(L["ErrorEvent"]);
            }

            ev.IsActive = isActive;
            await Repository.UpdateAsync(ev, true);
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", ev.IsActive);
        }

        private async Task IsEventSoldOut(long eventId)
        {
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.EventDates);
            var ev = query.FirstOrDefault(x => x.Id == eventId);

            if (ev == null)
            {
                throw new UserFriendlyException("Event not found.");
            }

            var hasTicketsAvailable = ev.EventDates
                .SelectMany(eventDate =>
                    ev.Prices.Select(async price =>
                    {
                        var ticketCount = await _ticketRepo.CountAsync(x =>
                            x.EventId == eventId &&
                            x.EventDateId == eventDate.Id &&
                            x.TicketCategoryId == price.TicketCategoryId &&
                            x.TicketSectorId == price.TicketSectorId &&
                            (x.TicketStatus == TicketStatus.Sold || x.TicketStatus == TicketStatus.Reserved));

                        return price.Count - ticketCount > 0;
                    }))
                .Any(task => task.Result);

            if (!hasTicketsAvailable)
            {
                ev.EventStatus = EventStatus.SoldOut;
            }

            await Repository.UpdateAsync(ev, true);
            await _hubContext.Clients.All.SendAsync("EventAvailabilityUpdate", ev.EventStatus);

            return;
        }

        private static string AddImageSizeParameters(string url, string imageType, bool isMobile)
        {
            if (string.IsNullOrEmpty(url) || url.Contains("?"))
            {
                return url;
            }

            switch (imageType)
            {
                case "PhotoGallery":
                    return $"{url}?width=1000&height=400";
                case "PhotoDetail":
                    return $"{url}?width=600&height=600";
                case "PhotoLogo":
                    return $"{url}?width=120&height=35";
                default:
                    return url;
            }
        }

        public async Task GetDashboardStatisticsAsync(long eventId)
        {
            var eve = await Repository.FirstOrDefaultAsync(x => x.Id == eventId);

            if (eve == null && !CurrentUser.Id.HasValue)
            {
                throw new UserFriendlyException(L["NonExistentEvent"]);
            }

            Guid currentUserIdToSend;

            if (CurrentUser.Id.HasValue)
            {
                currentUserIdToSend = CurrentUser.Id.Value;
            }
            else if (eve.CreatorId.HasValue)
            {
                currentUserIdToSend = eve.CreatorId.Value;
            }
            else
            {
                throw new UserFriendlyException(L["NonExistenCreator"]);
            }

            await _backgroundJobManager.EnqueueAsync(new DashboardStatisticsBackgroundJobArgs
            {
                CurrentUserId = currentUserIdToSend
            });
        }

        public async Task GetTicketsSoldAndAvailable(long eventId)
        {
            // Fetch the event with its event dates
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.EventDates);
            var eve = query.FirstOrDefault(x => x.Id == eventId);

            if (eve == null)
            {
                throw new UserFriendlyException(L["NonExistentEvent"]);
            }

            // Get the list of sold tickets for the event
            var soldTickets = await _ticketRepo.GetListAsync(x => x.EventId == eventId && x.TicketStatus == TicketStatus.Sold);

            // Calculate available and sold tickets for each price category and sector
            var result = eve.Prices
                .Select(price => new TicketCountModelDto
                {
                    TicketCategoryId = price.TicketCategoryId,
                    TicketCategoryName = price.TicketCategoryName,
                    TicketSectorId = price.TicketSectorId,
                    TicketSectorName = price.TicketSectorName,
                    Available = eve.EventDates.Sum(date => price.Count) - soldTickets.Count(t => t.TicketCategoryId == price.TicketCategoryId && t.TicketSectorId == price.TicketSectorId),
                    Sold = soldTickets.Count(t => t.TicketCategoryId == price.TicketCategoryId && t.TicketSectorId == price.TicketSectorId)
                })
                .ToList();

            // Get the list of purchases with state "Finish"
            var purchases = await _purchaseRepo.GetListAsync(x => x.State == PurchaseState.Finish);

            // Filter the purchases for the specific event
            var filteredPurchases = purchases
                .Where(p => p.Ticktes.Any(t => t.EventId == eventId))
                .ToList();

            // Find the most recent purchase with state "Finish"
            var lastPurchase = filteredPurchases.OrderByDescending(p => p.CreationTime).FirstOrDefault();
            DateTime? lastPurchaseDate = lastPurchase?.CreationTime;

            // Calculate the difference in minutes and round to integers
            var lastTimePurchase = "";
            if (lastPurchaseDate.HasValue)
            {
                var LastDatePurchase = DateTime.UtcNow - lastPurchaseDate.Value;

                if (LastDatePurchase.TotalMinutes < 60)
                {
                    int minutes = (int)Math.Ceiling(LastDatePurchase.TotalMinutes);
                    lastTimePurchase = minutes == 1 ? "1 " + L["Minute"] : $"{minutes} " + L["Minutes"];
                }
                else if (LastDatePurchase.TotalHours < 24)
                {
                    int hours = (int)Math.Ceiling(LastDatePurchase.TotalHours);
                    lastTimePurchase = hours == 1 ? "1 " + L["Hour"] : $"{hours} " + L["Hours"];
                }
                else
                {
                    int days = (int)Math.Ceiling(LastDatePurchase.TotalDays);
                    lastTimePurchase = days == 1 ? "1 " + L["Day"] : $"{days} " + L["Days"];
                }
            }

            // Create the statistics DTO
            var statistics = new TicketStatisticsDto
            {
                TicketCounts = result,
                LastTimePurchase = lastTimePurchase
            };                              

            // Send statistics to all clients using SignalR hub
            await _hubContext.Clients.All.SendAsync("TicketsSoldAndAvailable", statistics);
        }

        public async Task GetTicketsSoldAndAvailableForType(long eventId)
        {
            // Fetch the event with its event dates
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.EventDates);
            var eve = query.FirstOrDefault(x => x.Id == eventId);

            if (eve == null)
            {
                throw new UserFriendlyException(L["NonExistentEvent"]);
            }

            // Get the list of sold tickets for the event
            var soldTickets = await _ticketRepo.GetListAsync(x => x.EventId == eventId && x.TicketStatus == TicketStatus.Sold);

            // Calculate available and sold tickets for each price category and sector
            var result = eve.Prices
                .Select(price =>
                {
                    var sold = soldTickets.Count(t => t.TicketCategoryId == price.TicketCategoryId && t.TicketSectorId == price.TicketSectorId);

                    return new TicketCountModelDto
                    {
                        TicketCategoryId = price.TicketCategoryId,
                        TicketCategoryName = price.TicketCategoryName,
                        TicketSectorId = price.TicketSectorId,
                        TicketSectorName = price.TicketSectorName,
                        Price = price.Price,
                        Available = eve.EventDates.Sum(date => price.Count) - sold,
                        Sold = sold,
                        TotalSold = sold * price.Price
                    };
                })
                .ToList();


            // Calculate the minutes since the last purchase for each price category and sector
            var lastPurchaseDates = soldTickets
                .GroupBy(t => new { t.TicketCategoryId, t.TicketSectorId })
                .Select(g => new
                {
                    g.Key.TicketCategoryId,
                    g.Key.TicketSectorId,
                    LastPurchaseDate = g.Max(t => t.CreationTime)
                })
                .ToList();

            // Add minutes since last purchase to the result
            var ticketCountsWithMinutes = result.Select(r =>
            {
                var lastPurchaseDate = lastPurchaseDates
                    .Where(lp => lp.TicketCategoryId == r.TicketCategoryId && lp.TicketSectorId == r.TicketSectorId)
                    .Select(lp => lp.LastPurchaseDate)
                    .FirstOrDefault();

                if (lastPurchaseDate != DateTime.MinValue)
                {
                    var timeDifference = DateTime.UtcNow - lastPurchaseDate;

                    if (timeDifference.TotalMinutes < 60)
                    {
                        int minutes = (int)Math.Ceiling(timeDifference.TotalMinutes);
                        r.TimeLastPurchase = minutes == 1 ? "1 " + L["Minute"] : $"{minutes} " + L["Minutes"];
                    }
                    else if (timeDifference.TotalHours < 24)
                    {
                        int hours = (int)Math.Ceiling(timeDifference.TotalHours);
                        r.TimeLastPurchase = hours == 1 ? "1 " + L["Hour"] : $"{hours} " + L["Hours"];
                    }
                    else
                    {
                        int days = (int)Math.Ceiling(timeDifference.TotalDays);
                        r.TimeLastPurchase = days == 1 ? "1 " + L["Day"] : $"{days} " + L["Days"];
                    }
                }

                return r;
            }).ToList();

            // Create the statistics DTO
            var statistics = new TicketStatisticsDto
            {
                TicketCounts = ticketCountsWithMinutes
            };

            // Send statistics to all clients using SignalR hub
            await _hubContext.Clients.All.SendAsync("TicketsSoldAndAvailableForType", statistics);
        }

        public async Task GetTicketsStatistics(long eventId)
        {
            // Fetch the event with its event dates
            var query = await ReadOnlyRepository.WithDetailsAsync(x => x.EventDates);
            var eve = query.FirstOrDefault(x => x.Id == eventId);

            if (eve == null)
            {
                throw new UserFriendlyException(L["NonExistentEvent"]);
            }

            // Get the list of sold tickets for the event
            var soldTickets = await _ticketRepo.GetListAsync(x => x.EventId == eventId && x.TicketStatus == TicketStatus.Sold);

            // Calculate available, sold tickets, and total sold value for each price category and sector
            var result = eve.Prices
                .Select(price =>
                {
                    var sold = soldTickets.Count(t => t.TicketCategoryId == price.TicketCategoryId && t.TicketSectorId == price.TicketSectorId);
                    var total = eve.EventDates.Sum(date => price.Count);

                    return new
                    {
                        Available = total - sold,
                        Sold = sold,
                        Total = total,
                        TotalSold = sold * price.Price // Calculate total sold value
                    };
                })
                .ToList();

            // Aggregate the results to get the totals
            var totalTickets = result.Sum(r => r.Total);
            var soldTicketsTotal = result.Sum(r => r.Sold);
            var availableTickets = result.Sum(r => r.Available);
            var totalSoldValue = result.Sum(r => r.TotalSold);

            var ticketCount = new TicketCountModelDto
            {
                Available = availableTickets,
                Sold = soldTicketsTotal,
                Total = totalTickets,
                TotalSold = totalSoldValue
            };

            // Get the list of purchases with state "Finish"
            var purchases = await _purchaseRepo.GetListAsync(x => x.State == PurchaseState.Finish);

            // Filter the purchases for the specific event
            var filteredPurchases = purchases
                .Where(p => p.Ticktes.Any(t => t.EventId == eventId))
                .ToList();

            // Find the most recent purchase with state "Finish"
            var lastPurchase = filteredPurchases.OrderByDescending(p => p.CreationTime).FirstOrDefault();
            DateTime? lastPurchaseDate = lastPurchase?.CreationTime;

            // Calculate the difference in minutes and round to integers
            var lastTimePurchase = "";
            if (lastPurchaseDate.HasValue)
            {
                var LastDatePurchase = DateTime.UtcNow - lastPurchaseDate.Value;

                if (LastDatePurchase.TotalMinutes < 60)
                {
                    int minutes = (int)Math.Ceiling(LastDatePurchase.TotalMinutes);
                    lastTimePurchase = minutes == 1 ? "1 " + L["Minute"] : $"{minutes} " + L["Minutes"];
                }
                else if (LastDatePurchase.TotalHours < 24)
                {
                    int hours = (int)Math.Ceiling(LastDatePurchase.TotalHours);
                    lastTimePurchase = hours == 1 ? "1 " + L["Hour"] : $"{hours} " + L["Hours"];
                }
                else
                {
                    int days = (int)Math.Ceiling(LastDatePurchase.TotalDays);
                    lastTimePurchase = days == 1 ? "1 " + L["Day"] : $"{days} " + L["Days"];
                }
            }

            // Create the statistics DTO
            var statistics = new TicketStatisticsDto
            {
                TicketCounts = new List<TicketCountModelDto> { ticketCount },
                LastTimePurchase = lastTimePurchase
            };

            // Send statistics to all clients using SignalR hub
            await _hubContext.Clients.All.SendAsync("TicketStatistic", statistics);
        }

        public async Task UpdateEventDates(long eventId)
        {
            var eve = await Repository.FirstOrDefaultAsync(x => x.Id == eventId);

            if (eve == null)
            {
                throw new UserFriendlyException(L["NonExistentEvent"]);
            }

            if(!eve.BackgroundJobId.IsNullOrEmpty()) 
            {
                BackgroundJob.Delete(eve.BackgroundJobId);
            }

            var jobId = await _backgroundJobManager.EnqueueAsync(new UpdateEventDatesBackgroundJobArgs
            {
                EventId = eventId
            });

            eve.BackgroundJobId = jobId;

            await Repository.UpdateAsync(eve, true);
        }

        public async Task ShowUpdatedEventName(long eventId)
        {
            var eve = await Repository.FirstOrDefaultAsync(x => x.Id == eventId);

            if (eve == null)
            {
                throw new UserFriendlyException(L["NonExistentEvent"]);
            }

            await _hubContext.Clients.All.SendAsync("ShowUpdatedEventName", eve.Name);
        }
    }
}


