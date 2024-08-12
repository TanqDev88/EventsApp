import { mapEnumToOptions } from '@abp/ng.core';

export enum TypeUserEvent {
  Admin = 0,
  Editor = 1,
  Readonly = 2,
  Validator = 3,
}

export const typeUserEventOptions = mapEnumToOptions(TypeUserEvent);
