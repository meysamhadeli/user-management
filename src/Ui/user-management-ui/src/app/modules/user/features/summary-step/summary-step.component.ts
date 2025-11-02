import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCardModule } from '@angular/material/card';
import { RegistrationWizardService } from '../../services/registration-wizard.service';
import { IndustryDto } from '../../../industry/dtos/industry.dto';
import { IndustryService } from '../../../industry/services/industry.service ';

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
  private industryService = inject(IndustryService);

  registrationData: any;
  industries: IndustryDto[] = [];
  selectedIndustryName: string = '';

  constructor() {}

  ngOnInit(): void {
    this.registrationData = this.wizardService.getCurrentData();
    this.loadIndustries();
  }

  loadIndustries(): void {
    this.industryService.getIndustries().subscribe({
      next: (response) => {
        this.industries = response.items || [];
        this.findSelectedIndustryName();
      },
      error: (error) => {
        console.error('Failed to load industries:', error);
      }
    });
  }

  findSelectedIndustryName(): void {
    const selectedIndustry = this.industries.find(industry => 
      industry.id === this.registrationData.company.industryId
    );
    this.selectedIndustryName = selectedIndustry ? selectedIndustry.name : 'Unknown Industry';
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