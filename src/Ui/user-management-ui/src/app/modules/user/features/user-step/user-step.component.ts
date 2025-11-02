import { Component, Input, Output, EventEmitter, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { RegistrationWizardService } from '@/modules/user/services/registration-wizard.service';
import { UserService } from '@/modules/user/services/user.service';
import { CheckUsernameAvailabilityRequestDto } from '@/modules/user/dtos/check-username-availability-request.dto';
import { CheckEmailAvailabilityRequestDto } from '@/modules/user/dtos/check-email-availability-request.dto';


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
    MatProgressSpinnerModule
  ],
  templateUrl: './user-step.component.html'
})
export class UserStepComponent implements OnInit {
  @Input() userForm!: FormGroup;
  @Output() next = new EventEmitter<void>();
  @Output() previous = new EventEmitter<void>();

  private wizardService = inject(RegistrationWizardService);
  private userService = inject(UserService);

  checkingUsername = false;
  checkingEmail = false;
  hidePassword = true;
  hideConfirmPassword = true;

  constructor() {}

  ngOnInit(): void {
    this.setupUsernameAvailabilityCheck();
    this.setupEmailAvailabilityCheck();
  }

  private setupUsernameAvailabilityCheck(): void {
    this.userForm.get('userName')?.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(username => {
        if (this.userNameControl?.valid && username) {
          this.checkingUsername = true;
          const request: CheckUsernameAvailabilityRequestDto = { username };
          return this.userService.checkUsernameAvailability(request).pipe(
            catchError(() => {
              this.checkingUsername = false;
              return of(null);
            })
          );
        }
        this.checkingUsername = false;
        return of(null);
      })
    ).subscribe(response => {
      this.checkingUsername = false;
      if (response && !response.isAvailable) {
        this.userNameControl?.setErrors({ usernameTaken: true });
      }
    });
  }

  private setupEmailAvailabilityCheck(): void {
    this.userForm.get('email')?.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(email => {
        if (this.emailControl?.valid && email) {
          this.checkingEmail = true;
          const request: CheckEmailAvailabilityRequestDto = { email };
          return this.userService.checkEmailAvailability(request).pipe(
            catchError(() => {
              this.checkingEmail = false;
              return of(null);
            })
          );
        }
        this.checkingEmail = false;
        return of(null);
      })
    ).subscribe(response => {
      this.checkingEmail = false;
      if (response && !response.isAvailable) {
        this.emailControl?.setErrors({ emailTaken: true });
      }
    });
  }

  get userNameControl(): AbstractControl | null {
    return this.userForm.get('userName');
  }

  get emailControl(): AbstractControl | null {
    return this.userForm.get('email');
  }

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword = !this.hideConfirmPassword;
  }

  onSubmit(): void {
    if (this.userForm.valid) {
      this.wizardService.updateUserData(this.userForm.value);
      this.next.emit();
    }
  }

  onPrevious(): void {
    this.previous.emit();
  }
}