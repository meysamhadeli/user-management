import { Component, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { RegistrationWizardService } from '../../services/registration-wizard.service';
import { UserService } from '@/modules/user/services/user.service';
import { NotificationService } from '@/core/services/notification.service';

@Component({
  selector: 'app-completion-step',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCardModule
  ],
  templateUrl: './completion-step.component.html'
})
export class CompletionStepComponent implements OnInit {
  @Output() complete = new EventEmitter<void>();
  @Output() previous = new EventEmitter<void>();

  private wizardService = inject(RegistrationWizardService);
  private userService = inject(UserService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  isLoading = false;
  registrationData: any;

  ngOnInit(): void {
    this.registrationData = this.wizardService.getCurrentData();
  }

  onComplete(): void {
    this.isLoading = true;

    const registrationData = {
      ...this.registrationData.user,
      companyId: this.registrationData.company.companyId,
      acceptTermsOfService: this.registrationData.terms.acceptTermsOfService,
      acceptPrivacyPolicy: this.registrationData.terms.acceptPrivacyPolicy
    };

    this.userService.createUser(registrationData).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.notificationService.showSuccess('Registration completed successfully!');
        
        // Reset the wizard to clear all data
        this.wizardService.resetWizard();
        
        // Emit completion event
        this.complete.emit();
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Registration failed:', error);
        
        if (error.status === 409) {
          this.notificationService.showError('Username or email already exists. Please go back and choose different credentials.');
        } else {
          this.notificationService.showError('Registration failed. Please try again.');
        }
      }
    });
  }

  onPrevious(): void {
    this.previous.emit();
  }
}