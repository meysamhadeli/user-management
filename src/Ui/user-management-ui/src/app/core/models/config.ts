import { InjectionToken } from '@angular/core';

export interface AppConfig {
  apiHost: string;
  apiEndpoint?: string;
  production: boolean;
}

export const APP_CONFIG = new InjectionToken<AppConfig>('app.config');