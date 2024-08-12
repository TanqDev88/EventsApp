import type { EventDto, EventInputDto, EventResultRequestDto, PurchaseCheckDto, PurchaseDto, PurchaseResponseJsonDto, TicketAvailableDto, TicketValidateInputDto, TicketValidateOutDto, UserOutDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { PurchaseInputDto, PurchaseTicketDto, PurchaseTicketInputDto, TicketInputDto, TicketOutDto } from '../tickets/models';

@Injectable({
  providedIn: 'root',
})
export class EventService {
  apiName = 'Default';
  

  create = (input: EventInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EventDto>({
      method: 'POST',
      url: '/api/app/event',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/event/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EventDto>({
      method: 'GET',
      url: `/api/app/event/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByCodeByCode = (code: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EventDto>({
      method: 'GET',
      url: '/api/app/event/by-code',
      params: { code },
    },
    { apiName: this.apiName,...config });
  

  getDashboardStatistics = (eventId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: `/api/app/event/dashboard-statistics/${eventId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: EventResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<EventDto>>({
      method: 'GET',
      url: '/api/app/event',
      params: { userId: input.userId, keyword: input.keyword, eventType: input.eventType, includeFiles: input.includeFiles, isMain: input.isMain, isOwner: input.isOwner, isAdmin: input.isAdmin, isEditor: input.isEditor, isValidator: input.isValidator, isMobile: input.isMobile, eventStatus: input.eventStatus, dateFrom: input.dateFrom, dateTo: input.dateTo, order: input.order, excludeIds: input.excludeIds, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getTicketAvailableByInput = (input: TicketAvailableDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseTicketDto[]>({
      method: 'GET',
      url: '/api/app/event/ticket-available',
      params: { eventId: input.eventId, eventDateId: input.eventDateId },
    },
    { apiName: this.apiName,...config });
  

  getTicketByInput = (input: TicketInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TicketOutDto>({
      method: 'GET',
      url: '/api/app/event/ticket',
      params: { purchaseCode: input.purchaseCode, ticketId: input.ticketId },
    },
    { apiName: this.apiName,...config });
  

  getTicketCodeByInput = (input: TicketInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/event/ticket-code',
      params: { purchaseCode: input.purchaseCode, ticketId: input.ticketId },
    },
    { apiName: this.apiName,...config });
  

  getTicketsSoldAndAvailableByEventId = (eventId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: `/api/app/event/tickets-sold-and-available/${eventId}`,
    },
    { apiName: this.apiName,...config });
  

  getTicketsSoldAndAvailableForTypeByEventId = (eventId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: `/api/app/event/tickets-sold-and-available-for-type/${eventId}`,
    },
    { apiName: this.apiName,...config });
  

  getTicketsStatisticsByEventId = (eventId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'GET',
      url: `/api/app/event/tickets-statistics/${eventId}`,
    },
    { apiName: this.apiName,...config });
  

  getUsersEventByInput = (input: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserOutDto[]>({
      method: 'GET',
      url: '/api/app/event/users-event',
      params: { input },
    },
    { apiName: this.apiName,...config });
  

  puchaseCreateOrUpdateTicketByInput = (input: PurchaseTicketInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/event/puchase-create-or-update-ticket',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  purchaseCancelByCode = (code: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/event/purchase-cancel',
      params: { code },
    },
    { apiName: this.apiName,...config });
  

  purchaseCheckByInput = (input: PurchaseCheckDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/event/purchase-check',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  purchaseCreate = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/event/purchase-create',
    },
    { apiName: this.apiName,...config });
  

  purchaseGetByCode = (code: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseDto>({
      method: 'POST',
      url: '/api/app/event/purchase-get',
      params: { code },
    },
    { apiName: this.apiName,...config });
  

  purchaseProcessByInputAndInputCommissionAndSeconds = (input: PurchaseInputDto, inputCommission: number, seconds: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/event/purchase-process',
      params: { inputCommission, seconds },
      body: input,
    },
    { apiName: this.apiName,...config });
  

  purchaseResponseByInput = (input: PurchaseResponseJsonDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/event/purchase-response',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  showUpdatedEventNameByEventId = (eventId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/event/show-updated-event-name/${eventId}`,
    },
    { apiName: this.apiName,...config });
  

  ticketValidateByInput = (input: TicketValidateInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TicketValidateOutDto>({
      method: 'POST',
      url: '/api/app/event/ticket-validate',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  update = (id: number, input: EventInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EventDto>({
      method: 'PUT',
      url: `/api/app/event/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateEventDatesByEventId = (eventId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/event/event-dates/${eventId}`,
    },
    { apiName: this.apiName,...config });
  

  updateEventStatusByIdAndIsActive = (id: number, isActive: boolean, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/event/${id}/event-status`,
      params: { isActive },
    },
    { apiName: this.apiName,...config });
  

  validateCodeByCode = (code: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'POST',
      url: '/api/app/event/validate-code',
      params: { code },
    },
    { apiName: this.apiName,...config });
  

  validateTicketPurchase = (eventId: number, quantity: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'POST',
      url: `/api/app/event/validate-ticket-purchase/${eventId}`,
      params: { quantity },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
