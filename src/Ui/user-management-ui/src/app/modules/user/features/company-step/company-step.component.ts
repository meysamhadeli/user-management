import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RegistrationWizardService } from '../../services/registration-wizard.service';
import { IndustryDto } from '../../../industry/dtos/industry.dto';

@Component({
  selector: 'app-company-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './company-step.component.html'
})
export class CompanyStepComponent implements OnInit {
  @Input() industries: IndustryDto[] = [];
  @Input() companyForm!: FormGroup;
  @Output() next = new EventEmitter<void>();

  private wizardService = inject(RegistrationWizardService);

  constructor() {}

  ngOnInit(): void {
    // Form is now provided by parent component
  }

  onSubmit(): void {
    if (this.companyForm.valid) {
      this.wizardService.updateCompanyData(this.companyForm.value);
      this.next.emit();
    }
  }
}