// modules/user/features/summary-step/summary-step.component.ts
import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon'; // Add this import
import { RegistrationWizardService } from '../../services/registration-wizard.service';
import { IndustryDto } from '../../../industry/dtos/industry.dto';

@Component({
  selector: 'app-summary-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule
  ],
  templateUrl: './summary-step.component.html'
})
export class SummaryStepComponent {
  @Input() industries: IndustryDto[] = [];
  @Output() previous = new EventEmitter<void>();
  @Output() complete = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private wizardService = inject(RegistrationWizardService);

  termsForm: FormGroup;
  data = this.wizardService.getCurrentData();

  constructor() {
    this.termsForm = this.fb.group({
      acceptTermsOfService: [false, Validators.requiredTrue],
      acceptPrivacyPolicy: [false, Validators.requiredTrue]
    });
  }

  getIndustryName(industryId: string): string {
    const industry = this.industries.find(i => i.id === industryId);
    return industry ? industry.name : 'Unknown';
  }

  onPrevious(): void {
    this.wizardService.updateTermsData(this.termsForm.value);
    this.previous.emit();
  }

  onSubmit(): void {
    if (this.termsForm.valid) {
      this.wizardService.updateTermsData(this.termsForm.value);
      this.complete.emit();
    }
  }
}