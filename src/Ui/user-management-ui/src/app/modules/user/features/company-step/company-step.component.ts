import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Observable, of } from 'rxjs';
import { map, startWith, debounceTime, switchMap, catchError } from 'rxjs/operators';
import { CompanyDto } from '../../../company/dtos/company.dto';
import { CompanyService } from '../../../company/services/company.service';
import { IPageList } from '../../../../core/models/pagination';


@Component({
  selector: 'app-company-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './company-step.component.html'
})
export class CompanyStepComponent implements OnInit {
  @Input() label: string = 'Select Company';
  @Input() placeholder: string = 'Choose a company';
  @Input() required: boolean = false;
  @Input() preselectedCompanyId?: string;
  @Output() companySelected = new EventEmitter<CompanyDto>();

  private companyService = inject(CompanyService);

  companyControl = new FormControl('', this.required ? [Validators.required] : []);
  companies: CompanyDto[] = [];
  filteredCompanies$!: Observable<CompanyDto[]>;
  isLoading = false;
  searchError: string | null = null;
  selectedCompany: CompanyDto | null = null;

  ngOnInit(): void {
    this.setupAutocomplete();
    
    // Load initial companies for dropdown
    this.loadInitialCompanies();
  }

  private setupAutocomplete(): void {
    this.filteredCompanies$ = this.companyControl.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      switchMap(value => {
        const searchTerm = typeof value === 'string' ? value : '';
        
        if (searchTerm.length < 2) {
          this.searchError = null;
          return of(this.companies);
        }

        this.isLoading = true;
        this.searchError = null;
        
        return this.companyService.getCompanies(1, 20, `name.Contains("${searchTerm}")`, 'name').pipe(
          map(response => {
            this.isLoading = false;
            return response.items || [];
          }),
          catchError(error => {
            console.error('Search error:', error);
            this.isLoading = false;
            this.searchError = 'Failed to search companies. Please try again.';
            return of(this.companies);
          })
        );
      })
    );
  }

  private loadInitialCompanies(): void {
    this.isLoading = true;
    
    this.companyService.getCompanies(1, 50, null, 'name').subscribe({
      next: (response: IPageList<CompanyDto>) => {
        this.companies = response.items || [];
        this.isLoading = false;
        
        // If preselected company ID is provided, find and set it
        if (this.preselectedCompanyId) {
          const preselectedCompany = this.companies.find(c => c.id === this.preselectedCompanyId);
          if (preselectedCompany) {
            this.selectedCompany = preselectedCompany;
            this.companyControl.setValue(preselectedCompany.name); // Set the display name, not the object
            this.companySelected.emit(preselectedCompany);
          }
        }
      },
      error: (error) => {
        console.error('Failed to load companies:', error);
        this.isLoading = false;
        this.searchError = 'Failed to load companies. Please try again.';
      }
    });
  }

  displayCompany(company: CompanyDto): string {
    return company && company.name ? company.name : '';
  }

  onCompanySelected(company: CompanyDto): void {
    this.selectedCompany = company;
    this.companySelected.emit(company);
  }

  clearSelection(): void {
    this.companyControl.setValue('');
    this.selectedCompany = null;
    this.companySelected.emit(undefined as any);
  }

  // Helper method to check if the current value is a company object
  isCompanyObject(value: any): value is CompanyDto {
    return value && typeof value === 'object' && 'id' in value && 'name' in value;
  }
}