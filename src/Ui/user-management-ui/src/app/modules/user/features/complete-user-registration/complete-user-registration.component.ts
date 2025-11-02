import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { CompanyDto } from '../../../company/dtos/company.dto';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { CreateUserRequestDto } from '../../dtos/create-user-request.dto';
import { CompanyStepComponent } from '../company-step/company-step.component';

@Component({
  selector: 'app-complete-user-registration',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    CompanyStepComponent
  ],
  templateUrl: './complete-user-registration.component.html'
})
export class CompleteUserRegistrationComponent implements OnInit {
  registrationForm: FormGroup;
  selectedCompany: CompanyDto | null = null;
  isLoading = false;
  checkingUsername = false;
  hidePassword = true;
  hideConfirmPassword = true;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private userService: UserService,
    private notificationService: NotificationService
  ) {
    this.registrationForm = this.fb.group({
      companyId: ['', Validators.required],
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      userName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email: ['', [Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      passwordRepetition: ['', [Validators.required]],
      acceptTermsOfService: [false, [Validators.requiredTrue]],
      acceptPrivacyPolicy: [false, [Validators.requiredTrue]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    this.setupUsernameAvailabilityCheck();
  }

  private passwordMatchValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const password = control.get('password');
    const passwordRepetition = control.get('passwordRepetition');
    
    if (password && passwordRepetition && password.value !== passwordRepetition.value) {
      passwordRepetition.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  private setupUsernameAvailabilityCheck(): void {
    this.registrationForm.get('userName')?.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(username => {
        if (this.userNameControl?.valid && username) {
          this.checkingUsername = true;
          return this.userService.checkUsernameAvailability({ username }).pipe(
            catchError(() => {
              this.checkingUsername = false;
              return of(null);
            })
          );
        }
        this.checkingUsername = false;
        return of(null);
      })
    ).subscribe(response => {
      this.checkingUsername = false;
      if (response && !response.isAvailable) {
        this.userNameControl?.setErrors({ usernameTaken: true });
      }
    });
  }

  get userNameControl(): AbstractControl | null {
    return this.registrationForm.get('userName');
  }

  get passwordControl(): AbstractControl | null {
    return this.registrationForm.get('password');
  }

  get passwordRepetitionControl(): AbstractControl | null {
    return this.registrationForm.get('passwordRepetition');
  }

  onCompanySelected(company: CompanyDto): void {
    this.selectedCompany = company;
    this.registrationForm.patchValue({
      companyId: company.id
    });
    
    console.log('Selected company:', company);
  }

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword = !this.hideConfirmPassword;
  }

  onSubmit(): void {
    if (this.registrationForm.valid && this.selectedCompany) {
      this.isLoading = true;

      const userData: CreateUserRequestDto = {
        companyId: this.selectedCompany.id,
        firstName: this.registrationForm.get('firstName')?.value?.trim(),
        lastName: this.registrationForm.get('lastName')?.value?.trim(),
        userName: this.registrationForm.get('userName')?.value?.trim(),
        email: this.registrationForm.get('email')?.value?.trim() || undefined,
        password: this.registrationForm.get('password')?.value,
        passwordRepetition: this.registrationForm.get('passwordRepetition')?.value,
        acceptTermsOfService: this.registrationForm.get('acceptTermsOfService')?.value,
        acceptPrivacyPolicy: this.registrationForm.get('acceptPrivacyPolicy')?.value
      };

      this.userService.createUser(userData).subscribe({
        next: (response) => {
          console.log('User registration completed successfully:', response);
          this.isLoading = false;
          
          this.notificationService.showSuccess('User registered successfully!');
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          console.error('Failed to complete user registration:', error);
          this.isLoading = false;
          
          if (error.status === 400) {
            this.notificationService.showError('Invalid data. Please check your information and try again.');
          } else if (error.status === 409) {
            this.notificationService.showError('A user with this username already exists.');
          } else {
            this.notificationService.showError('Failed to complete registration. Please try again.');
          }
        }
      });
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.registrationForm.controls).forEach(key => {
        this.registrationForm.get(key)?.markAsTouched();
      });

      if (!this.selectedCompany) {
        this.notificationService.showWarning('Please select a company to continue.');
      } else {
        this.notificationService.showWarning('Please fill in all required fields correctly.');
      }
    }
  }

  onCancel(): void {
    this.router.navigate(['/']);
  }
}