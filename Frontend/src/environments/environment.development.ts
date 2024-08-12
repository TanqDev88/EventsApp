import { Environment } from '@abp/ng.core';

const baseUrl = 'https://ticketera.metrodev.ar';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'Ticketera',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://ticketera-api.metrodev.ar/',
    clientId: 'Ticketera_App',
    dummyClientSecret:'',
    scope: 'offline_access Ticketera'
  },
  apis: {
    default: {
      url: 'https://ticketera-api.metrodev.ar',
      rootNamespace: 'Ticketera',
    },
  },
} as Environment;
