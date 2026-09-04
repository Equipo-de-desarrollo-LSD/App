import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44392/',
  redirectUri: baseUrl,
  clientId: 'WayFinder_App',
  responseType: 'code',
  scope: 'offline_access WayFinder',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'WayFinder',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44392',
      rootNamespace: 'WayFinder',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
