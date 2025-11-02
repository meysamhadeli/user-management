export interface CreateUserRequestDto {
  companyId: string;
  firstName: string;
  lastName: string;
  userName: string;
  password: string;
  passwordRepetition: string;
  email?: string;
  acceptTermsOfService: boolean;
  acceptPrivacyPolicy: boolean;
}