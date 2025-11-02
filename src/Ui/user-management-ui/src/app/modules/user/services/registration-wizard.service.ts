import { Injectable } from '@angular/core';

export interface CompanyData {
  name: string;
  industryId: string;
}

export interface UserData {
  firstName: string;
  lastName: string;
  userName: string;
  password: string;
  passwordRepetition: string;
  email?: string;
}

export interface TermsData {
  acceptTermsOfService: boolean;
  acceptPrivacyPolicy: boolean;
}

export interface RegistrationData {
  company: CompanyData;
  user: UserData;
  terms: TermsData;
}

@Injectable({
  providedIn: 'root'
})
export class RegistrationWizardService {
  private currentStep = 1;
  private registrationData: RegistrationData = {
    company: {
      name: '',
      industryId: ''
    },
    user: {
      firstName: '',
      lastName: '',
      userName: '',
      password: '',
      passwordRepetition: '',
      email: ''
    },
    terms: {
      acceptTermsOfService: false,
      acceptPrivacyPolicy: false
    }
  };

  getCurrentStep(): number {
    return this.currentStep;
  }

  setCurrentStep(step: number): void {
    this.currentStep = step;
  }

  getCurrentData(): RegistrationData {
    return this.registrationData;
  }

  updateCompanyData(companyData: CompanyData): void {
    this.registrationData.company = { ...this.registrationData.company, ...companyData };
  }

  updateUserData(userData: UserData): void {
    this.registrationData.user = { ...this.registrationData.user, ...userData };
  }

  updateTermsData(termsData: TermsData): void {
    this.registrationData.terms = { ...this.registrationData.terms, ...termsData };
  }

  getRegistrationData(): RegistrationData {
    return this.registrationData;
  }

  resetWizard(): void {
    this.currentStep = 1;
    this.registrationData = {
      company: {
        name: '',
        industryId: ''
      },
      user: {
        firstName: '',
        lastName: '',
        userName: '',
        password: '',
        passwordRepetition: '',
        email: ''
      },
      terms: {
        acceptTermsOfService: false,
        acceptPrivacyPolicy: false
      }
    };
  }
}