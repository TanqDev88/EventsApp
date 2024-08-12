import { mapEnumToOptions } from '@abp/ng.core';

export enum PurchaseState {
  Pending = 0,
  Cancel = 1,
  Finish = 2,
  InReview = 3,
  InProcess = 4,
}

export const purchaseStateOptions = mapEnumToOptions(PurchaseState);
