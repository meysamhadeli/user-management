// app.routes.ts
import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/user/register', pathMatch: 'full' },
  
  // Industry Routes
  { 
    path: 'industry/create', 
    loadComponent: () => import('./modules/industry/features/create-industry/create-industry.component').then(m => m.CreateIndustryComponent) 
  },
  
  // Company Routes
  { 
    path: 'company/create', 
    loadComponent: () => import('./modules/company/features/create-company/create-company.component').then(m => m.CreateCompanyComponent) 
  },
  
  // User Routes
  { 
    path: 'user/register', 
    loadComponent: () => import('./modules/user/features/registration-wizard/registration-wizard.component').then(m => m.RegistrationWizardComponent) 
  },
  
  { path: '**', redirectTo: '/user/register' }
];