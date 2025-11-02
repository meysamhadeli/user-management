import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { IndustryDto } from '../../../industry/dtos/industry.dto';
import { NotificationService } from '../../../../core/services/notification.service';
import { IPageList } from '../../../../core/models/pagination';
import { IndustryService } from '../../../industry/services/industry.service ';
import { CompanyService } from '../../services/company.service';
import { CreateCompanyRequestDto } from '../../dtos/create-company-request.dto';

@Component({
  selector: 'app-create-company',
  templateUrl: './create-company.component.html',
  styleUrls: ['./create-company.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ]
})
export class CreateCompanyComponent implements OnInit {
  companyForm: FormGroup;
  industries: IndustryDto[] = [];
  loadingIndustries = true;
  isLoading = false;
  industriesError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private industryService: IndustryService,
    private companyService: CompanyService,
    private notificationService: NotificationService
  ) {
    this.companyForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      industryId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadIndustries();
  }

  loadIndustries(): void {
    this.loadingIndustries = true;
    this.industriesError = null;

    this.industryService.getIndustries().subscribe({
      next: (response: IPageList<IndustryDto>) => {
        this.industries = response.items || [];
        this.loadingIndustries = false;

        if (this.industries.length === 0) {
          this.notificationService.showInfo('No industries available. Please create an industry first.');
        }
      },
      error: (error) => {
        console.error('Failed to load industries:', error);
        this.industriesError = 'Failed to load industries. Please try again.';
        this.loadingIndustries = false;
        this.industries = [];
        
        this.notificationService.showError('Failed to load industries. Please try again.');
      }
    });
  }

  onSubmit(): void {
    if (this.companyForm.valid) {
      this.isLoading = true;

      const companyData: CreateCompanyRequestDto = {
        name: this.companyForm.get('name')?.value?.trim(),
        industryId: this.companyForm.get('industryId')?.value // Keep as string
      };

      this.companyService.createCompany(companyData).subscribe({
        next: (response) => {
          console.log('Company created successfully:', response);
          this.isLoading = false;
          
          // Success message will be shown by the ApiService via the OperationDescriptor
          this.router.navigate(['/companies']);
        },
        error: (error) => {
          console.error('Failed to create company:', error);
          this.isLoading = false;
          
          // Error message will be shown by the ApiService via the OperationDescriptor
          // Additional error handling if needed
          if (error.status === 400) {
            this.notificationService.showWarning('Please check your input and try again.');
          }
        }
      });
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.companyForm.controls).forEach(key => {
        this.companyForm.get(key)?.markAsTouched();
      });
      
      this.notificationService.showWarning('Please fill in all required fields correctly.');
    }
  }

  retryLoadIndustries(): void {
    this.loadIndustries();
  }
}