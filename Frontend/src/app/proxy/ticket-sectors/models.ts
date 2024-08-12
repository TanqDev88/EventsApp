import type { EntityDto, PagedResultRequestDto } from '@abp/ng.core';

export interface GetTicketSectorListDto extends PagedResultRequestDto {
  keyword?: string;
}

export interface TicketSectorDto extends EntityDto<number> {
  name?: string;
  description?: string;
  creatorId?: string;
}
