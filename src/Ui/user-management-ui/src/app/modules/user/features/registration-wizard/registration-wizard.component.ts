import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatStepperModule, MatStepper } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CompanyStepComponent } from '../company-step/company-step.component';
import { UserStepComponent } from '../user-step/user-step.component';
import { SummaryStepComponent } from '../summary-step/summary-step.component';
import { CompletionStepComponent } from '../completion-step/completion-step.component';
import { RegistrationWizardService } from '../../services/registration-wizard.service';

@Component({
  selector: 'app-registration-wizard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatStepperModule,
    MatButtonModule,
    MatIconModule,
    CompanyStepComponent,
    UserStepComponent,
    SummaryStepComponent,
    CompletionStepComponent
  ],
  templateUrl: './registration-wizard.component.html'
})
export class RegistrationWizardComponent implements OnInit {
  @ViewChild('stepper') stepper!: MatStepper;

  private fb = inject(FormBuilder);
  private wizardService = inject(RegistrationWizardService);
  private router = inject(Router);

  // Form groups for each step (required for linear stepper)
  companyStepForm: FormGroup;
  userStepForm: FormGroup;
  termsStepForm: FormGroup;

  constructor() {
    // Initialize form groups for each step
    this.companyStepForm = this.fb.group({
      name: ['', [Validators.required]],
      companyId: ['', [Validators.required]],
      industryId: ['', [Validators.required]],
      industryName: [''] // Add industryName field
    });

    this.userStepForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      userName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]], // Email is now required
      password: ['', [Validators.required, Validators.minLength(6)]],
      passwordRepetition: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });

    this.termsStepForm = this.fb.group({
      acceptTermsOfService: [false, [Validators.requiredTrue]],
      acceptPrivacyPolicy: [false, [Validators.requiredTrue]]
    });
  }

  ngOnInit(): void {
    this.loadExistingData();
  }

  private passwordMatchValidator(control: FormGroup): { [key: string]: boolean } | null {
    const password = control.get('password');
    const passwordRepetition = control.get('passwordRepetition');
    
    if (password && passwordRepetition && password.value !== passwordRepetition.value) {
      passwordRepetition.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  loadExistingData(): void {
    const data = this.wizardService.getCurrentData();
    
    // Patch existing data into forms
    this.companyStepForm.patchValue(data.company);
    this.userStepForm.patchValue(data.user);
    this.termsStepForm.patchValue(data.terms);
  }

  onCompanyStepComplete(): void {
    if (this.companyStepForm.valid) {
      this.wizardService.updateCompanyData(this.companyStepForm.value);
      this.stepper.next();
    }
  }

  onUserStepComplete(): void {
    if (this.userStepForm.valid) {
      this.wizardService.updateUserData(this.userStepForm.value);
      this.stepper.next();
    }
  }

  onTermsStepComplete(): void {
    if (this.termsStepForm.valid) {
      this.wizardService.updateTermsData(this.termsStepForm.value);
      this.stepper.next();
    }
  }

  onRegistrationComplete(): void {
    this.wizardService.resetWizard();
    this.router.navigate(['/login']);
  }

  onStepChange(event: any): void {
    const stepIndex = event.selectedIndex;
    this.wizardService.setCurrentStep(stepIndex + 1);
  }

  onCancel(): void {
    this.wizardService.resetWizard();
    this.router.navigate(['/']);
  }
}