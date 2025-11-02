import { Component, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { RegistrationWizardService } from '../../services/registration-wizard.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule
  ],
  templateUrl: './user-step.component.html'
})
export class UserStepComponent {
  @Output() previous = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private wizardService = inject(RegistrationWizardService);
  private userService = inject(UserService);

  userForm: FormGroup;
  hidePassword = true;
  hidePasswordRepetition = true;

  constructor() {
    this.userForm = this.createForm();
    const data = this.wizardService.getCurrentData();
    this.userForm.patchValue(data.user);
  }

  private createForm(): FormGroup {
    return this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      userName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email: [''],
      password: ['', [Validators.required, Validators.minLength(6)]],
      passwordRepetition: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  private passwordMatchValidator(control: AbstractControl) {
    const password = control.get('password')?.value;
    const passwordRepetition = control.get('passwordRepetition')?.value;
    return password && passwordRepetition && password === passwordRepetition ? null : { passwordMismatch: true };
  }

  onPrevious(): void {
    this.wizardService.updateUserData(this.userForm.value);
    this.previous.emit();
  }

  onSubmit(): void {
    if (this.userForm.valid) {
      this.wizardService.updateUserData(this.userForm.value);
      this.next.emit();
    }
  }
}