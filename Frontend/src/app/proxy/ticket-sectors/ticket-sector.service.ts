import type { GetTicketSectorListDto, TicketSectorDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TicketSectorService {
  apiName = 'Default';
  

  create = (input: TicketSectorDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TicketSectorDto>({
      method: 'POST',
      url: '/api/app/ticket-sector',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/ticket-sector/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TicketSectorDto>({
      method: 'GET',
      url: `/api/app/ticket-sector/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetTicketSectorListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<TicketSectorDto>>({
      method: 'GET',
      url: '/api/app/ticket-sector',
      params: { keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: number, input: TicketSectorDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TicketSectorDto>({
      method: 'PUT',
      url: `/api/app/ticket-sector/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
