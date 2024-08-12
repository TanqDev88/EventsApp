import { mapEnumToOptions } from '@abp/ng.core';

export enum TicketValidateType {
  Success = 0,
  Fail = 1,
  Already = 2,
}

export const ticketValidateTypeOptions = mapEnumToOptions(TicketValidateType);
