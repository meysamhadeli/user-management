import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCardModule } from '@angular/material/card';
import { RegistrationWizardService } from '@/modules/user/services/registration-wizard.service';

@Component({
  selector: 'app-summary-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatCardModule
  ],
  templateUrl: './summary-step.component.html'
})
export class SummaryStepComponent implements OnInit {
  @Input() termsForm!: FormGroup;
  @Output() next = new EventEmitter<void>();
  @Output() previous = new EventEmitter<void>();

  private wizardService = inject(RegistrationWizardService);

  registrationData: any;

  constructor() {}

  ngOnInit(): void {
    this.registrationData = this.wizardService.getCurrentData();
    console.log('Registration Data in Summary:', this.registrationData);
  }

  getIndustryName(): string {
    // Use the stored industryName from company step
    return this.registrationData.company.industryName || 'Unknown Industry';
  }

  getCompanyName(): string {
    return this.registrationData.company.name || 'Unknown Company';
  }

  onSubmit(): void {
    if (this.termsForm.valid) {
      this.wizardService.updateTermsData(this.termsForm.value);
      this.next.emit();
    }
  }

  onPrevious(): void {
    this.previous.emit();
  }
}