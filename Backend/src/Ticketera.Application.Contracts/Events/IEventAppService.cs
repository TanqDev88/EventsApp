using System.Collections.Generic;
using System.Threading.Tasks;
using Ticketera.Tickets;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Ticketera.Events
{
    public interface IEventAppService: ICrudAppService<EventDto, long, EventResultRequestDto, EventInputDto>
    {
        Task<string> PurchaseCreate();
        Task PurchaseCancel(string purchaseId);
        Task PuchaseCreateOrUpdateTicket(PurchaseTicketInputDto input);
        Task<IList<PurchaseTicketDto>> GetTicketAvailable(TicketAvailableDto input);
        Task<string> PurchaseProcess(PurchaseInputDto input, decimal inputCommission, int seconds);
        Task<string?> PurchaseCheck(PurchaseCheckDto input);
        Task PurchaseResponse(PurchaseResponseJsonDto input);
        Task<PurchaseDto> PurchaseGet(string id);
        Task<IRemoteStreamContent?> GetTicketCode(TicketInputDto input);
        Task<TicketOutDto> GetTicket(TicketInputDto input);
        Task<TicketValidateOutDto> TicketValidate(TicketValidateInputDto input);
        Task<IList<UserOutDto>> GetUsersEvent(string input);
        Task<bool> ValidateCode(string code);
        Task<bool> ValidateTicketPurchaseAsync(long eventId, int quantity);
        Task UpdateEventStatus(long id, bool isActive);
        Task GetTicketsSoldAndAvailable(long eventId);
        Task GetDashboardStatisticsAsync(long eventId);
        Task GetTicketsSoldAndAvailableForType(long eventId);
        Task GetTicketsStatistics(long eventId);
        Task UpdateEventDates(long eventId);
        Task ShowUpdatedEventName(long eventId);
    }
}