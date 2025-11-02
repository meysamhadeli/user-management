// app.config.ts
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from '@/app.routes';
import { APP_CONFIG } from '@/core/models/config';

// Material imports for services
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { environment } from './environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    provideAnimations(), // Required for Material animations
    { provide: APP_CONFIG, useValue: environment },
    importProvidersFrom(MatSnackBarModule)
  ]
};