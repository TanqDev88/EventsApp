
export interface PurchaseInputDto {
  code?: string;
  eventId: number;
  eventDateId: number;
  name?: string;
  surname?: string;
  email?: string;
  phone?: string;
}

export interface PurchaseTicketDto {
  count: number;
  ticketCategoryId?: number;
  ticketSectorId?: number;
  price: number;
  ticketCategoryName?: string;
  ticketSectorName?: string;
}

export interface PurchaseTicketInputDto {
  count: number;
  code?: string;
  eventId: number;
  eventDateId: number;
  ticketCategoryId?: number;
  ticketSectorId?: number;
}

export interface TicketInputDto {
  purchaseCode?: string;
  ticketId: number;
}

export interface TicketOutDto {
  title?: string;
  price: number;
  ticketCategoryName?: string;
  ticketSectorName?: string;
  eventDateStartDate?: string;
  eventPlace?: string;
  soldUsed: boolean;
}
