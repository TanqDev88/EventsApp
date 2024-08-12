import type { GetTicketCategoryListDto, TicketCategoryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TicketCategoryService {
  apiName = 'Default';
  

  create = (input: TicketCategoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TicketCategoryDto>({
      method: 'POST',
      url: '/api/app/ticket-category',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/ticket-category/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TicketCategoryDto>({
      method: 'GET',
      url: `/api/app/ticket-category/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetTicketCategoryListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<TicketCategoryDto>>({
      method: 'GET',
      url: '/api/app/ticket-category',
      params: { keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: number, input: TicketCategoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TicketCategoryDto>({
      method: 'PUT',
      url: `/api/app/ticket-category/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
