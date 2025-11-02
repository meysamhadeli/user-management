import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { IndustryService } from '../../services/industry.service ';
import { NotificationService } from '../../../../core/services/notification.service';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-create-industry',
  standalone: true,
  imports: [ 
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './create-industry.component.html',
  styleUrls: ['./create-industry.component.scss']
})
export class CreateIndustryComponent {
  private fb = inject(FormBuilder);
  private industryService = inject(IndustryService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  industryForm: FormGroup;
  isLoading = false;

  constructor() {
    this.industryForm = this.createForm();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]]
    });
  }

  onSubmit(): void {
    if (this.industryForm.valid) {
      this.isLoading = true;
      
      this.industryService.createIndustry(this.industryForm.value).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.notificationService.showSuccess('Industry created successfully!');
          this.router.navigate(['/']);
        },
        error: (error) => {
          this.isLoading = false;
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.industryForm.controls).forEach(key => {
      this.industryForm.get(key)?.markAsTouched();
    });
  }
}