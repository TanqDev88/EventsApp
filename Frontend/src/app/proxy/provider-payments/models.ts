import type { EntityDto, PagedResultRequestDto } from '@abp/ng.core';

export interface GetProviderPaymentListDto extends PagedResultRequestDto {
  keyword?: string;
}

export interface ProviderPaymentDto extends EntityDto<number> {
  name?: string;
  description?: string;
  creatorId?: string;
}
