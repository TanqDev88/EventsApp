import type { BaseAuditedDto } from '../base/models';
import type { EventType } from '../enum/event-type.enum';
import type { EventStatus } from '../enum/event-status.enum';
import type { TypeUserEvent } from '../enum/type-user-event.enum';
import type { PagedResultRequestDto } from '@abp/ng.core';
import type { PurchaseState } from '../enum/purchase-state.enum';
import type { TicketValidateType } from '../enum/ticket-validate-type.enum';

export interface EventDateDto {
  id: number;
  startDate?: string;
  endDate?: string;
  eventId: number;
}

export interface EventDto extends BaseAuditedDto {
  name?: string;
  description?: string;
  eventType: EventType;
  code?: string;
  bgColor?: string;
  photoGallery?: string;
  photoDetail?: string;
  photoLogo?: string;
  idPhotoLogo?: string;
  ticketsCount: number;
  eventDatesCount: number;
  userEventsCount: number;
  fileAttachmentsCount: number;
  isOwner: boolean;
  isAdmin: boolean;
  isEditor: boolean;
  isValidator: boolean;
  isActive: boolean;
  isMain: boolean;
  eventStatus: EventStatus;
  place?: string;
  eventDates: EventDateDto[];
  prices: PriceDto[];
  idProviderPayment: number[];
  saleForPerson: number;
  userEvents: UserEventDto[];
}

export interface EventInputDto extends BaseAuditedDto {
  name?: string;
  description?: string;
  eventType: EventType;
  code?: string;
  bgColor?: string;
  photoGallery: number;
  photoDetail: number;
  photoLogo: number;
  isMain: boolean;
  isActive: boolean;
  eventStatus: EventStatus;
  place?: string;
  eventDatesInput: EventDateDto[];
  prices: PriceDto[];
  idProviderPayment: number[];
  validators: Record<string, TypeUserEvent>;
  saleForPerson: number;
}

export interface EventResultRequestDto extends PagedResultRequestDto {
  userId?: string;
  keyword?: string;
  eventType?: EventType;
  includeFiles: boolean;
  isMain?: boolean;
  isOwner: boolean;
  isAdmin: boolean;
  isEditor: boolean;
  isValidator: boolean;
  isMobile: boolean;
  eventStatus: EventStatus;
  dateFrom?: string;
  dateTo?: string;
  order?: string;
  excludeIds: number[];
}

export interface PriceDto {
  count: number;
  price: number;
  ticketCategoryId: number;
  ticketCategoryName?: string;
  ticketSectorId: number;
  ticketSectorName?: string;
}

export interface PurchaseCheckDto {
  collection_id?: string;
  collection_status?: string;
  payment_id?: string;
  status?: string;
  external_reference?: string;
  payment_type?: string;
  merchant_order_id?: string;
  preference_id?: string;
  site_id?: string;
  processing_mode?: string;
  merchant_account_id?: string;
}

export interface PurchaseDto {
  id: number;
  name?: string;
  surname?: string;
  email?: string;
  phone?: string;
  state: PurchaseState;
}

export interface PurchaseResponseJsonDataDto {
  id?: string;
}

export interface PurchaseResponseJsonDto {
  resource?: string;
  topic?: string;
  action?: string;
  api_version?: string;
  data: PurchaseResponseJsonDataDto;
  date_created?: string;
  id: number;
  live_mode: boolean;
  type?: string;
  user_id?: string;
}

export interface TicketAvailableDto {
  eventId: number;
  eventDateId: number;
}

export interface TicketValidateInputDto {
  eventId: number;
  ticketCode?: string;
}

export interface TicketValidateOutDto {
  clientName?: string;
  categoryName?: string;
  sectorName?: string;
  status: TicketValidateType;
}

export interface UserEventDto {
  identityUserEmail?: string;
  typeUserEvent: TypeUserEvent;
  identityUserId?: string;
}

export interface UserOutDto {
  id?: string;
  name?: string;
  surname?: string;
  email?: string;
}
