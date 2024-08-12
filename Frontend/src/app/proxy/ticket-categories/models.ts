import type { EntityDto, PagedResultRequestDto } from '@abp/ng.core';

export interface GetTicketCategoryListDto extends PagedResultRequestDto {
  keyword?: string;
}

export interface TicketCategoryDto extends EntityDto<number> {
  name?: string;
  description?: string;
  creatorId?: string;
}
