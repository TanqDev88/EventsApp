import { Environment } from '@abp/ng.core';

const baseUrl = 'https://tixgo.mx';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'Ticketera',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://api.tixgo.mx/',
    clientId: 'Ticketera_App',
    dummyClientSecret:'',
    scope: 'offline_access Ticketera'
  },
  apis: {
    default: {
      url: 'https://api.tixgo.mx',
      rootNamespace: 'Ticketera',
    },
  },
} as Environment;
