import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'Ticketera',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44311/',
    clientId: 'Ticketera_App',
    dummyClientSecret:'',
    scope: 'offline_access Ticketera'
  },
  apis: {
    default: {
      url: 'https://localhost:44311',
      rootNamespace: 'Ticketera',
    },
  },
} as Environment;
