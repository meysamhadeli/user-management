import { Injectable } from '@angular/core';

export interface CompanyData {
  name: string;
  companyId: string; 
  industryId: string; 
  industryName: string; 
}

export interface UserData {
  firstName: string;
  lastName: string;
  userName: string;
  password: string;
  passwordRepetition: string;
  email: string;
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
      companyId: '', // Initialize companyId
      industryId: '', // Initialize industryId
      industryName: '' // Initialize industryName
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

  updateCompanyData(companyData: Partial<CompanyData>): void {
    this.registrationData.company = { ...this.registrationData.company, ...companyData };
    console.log('Updated company data:', this.registrationData.company);
  }

  updateUserData(userData: Partial<UserData>): void {
    this.registrationData.user = { ...this.registrationData.user, ...userData };
  }

  updateTermsData(termsData: Partial<TermsData>): void {
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
        companyId: '',
        industryId: '',
        industryName: ''
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