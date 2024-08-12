import { mapEnumToOptions } from '@abp/ng.core';

export enum EventStatus {
  Available = 0,
  Finalized = 1,
  SoldOut = 2,
}

export const eventStatusOptions = mapEnumToOptions(EventStatus);
