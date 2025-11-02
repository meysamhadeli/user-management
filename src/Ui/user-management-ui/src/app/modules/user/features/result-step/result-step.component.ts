import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { RegistrationWizardService } from '../../services/registration-wizard.service';

@Component({
  selector: 'app-result-step',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './result-step.component.html'
})
export class ResultStepComponent {
  private wizardService = inject(RegistrationWizardService);
  private router = inject(Router);

  onNewRegistration(): void {
    this.wizardService.reset();
    this.router.navigate(['/user/register']);
  }
}