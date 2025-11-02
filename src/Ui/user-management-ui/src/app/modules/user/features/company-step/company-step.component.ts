import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subscription } from 'rxjs';
import { RegistrationWizardService } from '@/modules/user/services/registration-wizard.service';
import { CompanyService } from '@/modules/company/services/company.service';
import { CompanyDto } from '@/modules/company/dtos/company.dto';

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
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './company-step.component.html'
})
export class CompanyStepComponent implements OnInit, OnDestroy {
  @Input() companyForm!: FormGroup;
  @Output() next = new EventEmitter<void>();

  private wizardService = inject(RegistrationWizardService);
  private companyService = inject(CompanyService);
  private formResetSubscription!: Subscription;

  companies: CompanyDto[] = [];
  selectedCompany: CompanyDto | null = null;
  isLoading = false;

  constructor() {}

  ngOnInit(): void {
    this.loadCompanies();
    
    // Subscribe to form reset events from parent
    this.formResetSubscription = this.companyForm.valueChanges.subscribe(value => {
      // If all company form values are empty, clear the selection
      if (!value.name && !value.companyId && !value.industryId && !value.industryName) {
        this.clearSelection();
      }
    });
  }

  ngOnDestroy(): void {
    if (this.formResetSubscription) {
      this.formResetSubscription.unsubscribe();
    }
  }

  loadCompanies(): void {
    this.isLoading = true;
    
    this.companyService.getCompanies(1, 100, null, 'name').subscribe({
      next: (response) => {
        this.companies = response.items || [];
        this.isLoading = false;
        
        // Load existing data if any
        const data = this.wizardService.getCurrentData();
        if (data.company.companyId) {
          const preselectedCompany = this.companies.find(c => c.id === data.company.companyId);
          if (preselectedCompany) {
            this.onCompanySelected(preselectedCompany);
          }
        }
      },
      error: (error) => {
        console.error('Failed to load companies:', error);
        this.isLoading = false;
      }
    });
  }

  onCompanySelected(company: CompanyDto): void {
    this.selectedCompany = company;
    
    const industryName = this.getIndustryName();
    
    // Update the form with selected company data
    this.companyForm.patchValue({
      name: company.name,
      companyId: company.id,
      industryId: company.id,
      industryName: industryName
    });

    // Update wizard service with complete company data
    this.wizardService.updateCompanyData({
      name: company.name,
      companyId: company.id,
      industryId: company.id,
      industryName: industryName
    });
    
    console.log('Company selected and stored:', {
      name: company.name,
      companyId: company.id,
      industryId: company.id,
      industryName: industryName
    });
  }

  clearSelection(): void {
    this.selectedCompany = null;
    this.companyForm.patchValue({
      name: '',
      companyId: '',
      industryId: '',
      industryName: ''
    });
    
    // Also clear the wizard service data
    this.wizardService.updateCompanyData({
      name: '',
      companyId: '',
      industryId: '',
      industryName: ''
    });
  }

  onSubmit(): void {
    if (this.companyForm.valid && this.selectedCompany) {
      console.log('Submitting company data:', this.companyForm.value);
      this.next.emit();
    }
  }

  // Helper method to get industry name safely
  getIndustryName(): string {
    return this.selectedCompany?.industry?.name || 'Not specified';
  }

  // Helper method to get industry description safely
  getIndustryDescription(): string {
    return this.selectedCompany?.industry?.description || 'No description available';
  }
}