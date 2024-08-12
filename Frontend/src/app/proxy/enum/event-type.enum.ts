import { mapEnumToOptions } from '@abp/ng.core';

export enum EventType {
  Public = 0,
  Private = 1,
}

export const eventTypeOptions = mapEnumToOptions(EventType);
