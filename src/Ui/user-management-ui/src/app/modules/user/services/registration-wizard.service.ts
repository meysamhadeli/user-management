import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface RegistrationWizardData {
  currentStep: number;
  company: {
    name: string;
    industryId: string;
  };
  user: {
    firstName: string;
    lastName: string;
    userName: string;
    password: string;
    passwordRepetition: string;
    email?: string;
  };
  terms: {
    acceptTermsOfService: boolean;
    acceptPrivacyPolicy: boolean;
  };
}

@Injectable({
  providedIn: 'root'
})
export class RegistrationWizardService {
  private data: RegistrationWizardData = {
    currentStep: 1,
    company: { name: '', industryId: '' },
    user: { firstName: '', lastName: '', userName: '', password: '', passwordRepetition: '', email: '' },
    terms: { acceptTermsOfService: false, acceptPrivacyPolicy: false }
  };

  private dataSubject = new BehaviorSubject<RegistrationWizardData>(this.data);
  public data$ = this.dataSubject.asObservable();

  updateCompanyData(companyData: Partial<RegistrationWizardData['company']>): void {
    this.data.company = { ...this.data.company, ...companyData };
    this.dataSubject.next(this.data);
  }

  updateUserData(userData: Partial<RegistrationWizardData['user']>): void {
    this.data.user = { ...this.data.user, ...userData };
    this.dataSubject.next(this.data);
  }

  updateTermsData(termsData: Partial<RegistrationWizardData['terms']>): void {
    this.data.terms = { ...this.data.terms, ...termsData };
    this.dataSubject.next(this.data);
  }

  setCurrentStep(step: number): void {
    this.data.currentStep = step;
    this.dataSubject.next(this.data);
  }

  getCurrentData(): RegistrationWizardData {
    return { ...this.data };
  }

  reset(): void {
    this.data = {
      currentStep: 1,
      company: { name: '', industryId: '' },
      user: { firstName: '', lastName: '', userName: '', password: '', passwordRepetition: '', email: '' },
      terms: { acceptTermsOfService: false, acceptPrivacyPolicy: false }
    };
    this.dataSubject.next(this.data);
  }
}