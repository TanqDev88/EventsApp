import type { GetProviderPaymentListDto, ProviderPaymentDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProviderPaymentService {
  apiName = 'Default';
  

  create = (input: ProviderPaymentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProviderPaymentDto>({
      method: 'POST',
      url: '/api/app/provider-payment',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/provider-payment/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProviderPaymentDto>({
      method: 'GET',
      url: `/api/app/provider-payment/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetProviderPaymentListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProviderPaymentDto>>({
      method: 'GET',
      url: '/api/app/provider-payment',
      params: { keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: number, input: ProviderPaymentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProviderPaymentDto>({
      method: 'PUT',
      url: `/api/app/provider-payment/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
