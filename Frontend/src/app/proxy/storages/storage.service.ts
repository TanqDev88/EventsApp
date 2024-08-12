import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StorageService {
  apiName = 'Default';
  

  getFileByFileId = (fileId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/storage/file/${fileId}`,
    },
    { apiName: this.apiName,...config });
  

  getFileByFileName = (fileName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/storage/file',
      params: { fileName },
    },
    { apiName: this.apiName,...config });
  

  getImageByFileName = (fileName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/storage/image',
      params: { fileName },
    },
    { apiName: this.apiName,...config });
  

  postFileByFile = (file: FormData, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'POST',
      url: '/api/app/storage/file',
      body: file,
    },
    { apiName: this.apiName,...config });
  

  postImageByFile = (file: FormData, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'POST',
      url: '/api/app/storage/image',
      body: file,
    },
    { apiName: this.apiName,...config });
  

  removeFileByFileId = (fileId: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'DELETE',
      url: `/api/app/storage/file/${fileId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
