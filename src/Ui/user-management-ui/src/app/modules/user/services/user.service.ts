import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateUserResponseDto } from '../dtos/create-user-response.dto';
import { CreateUserRequestDto } from '../dtos/create-user-request.dto';
import { HttpParams } from '@angular/common/http';
import { CheckEmailAvailabilityRequestDto } from '../dtos/check-email-availability-request.dto';
import { CheckEmailAvailabilityResponseDto } from '../dtos/check-email-availability-response.dto';
import { CheckUsernameAvailabilityRequestDto } from '../dtos/check-username-availability-request.dto';
import { CheckUsernameAvailabilityResponseDto } from '../dtos/check-username-availability-response.dto';
import { ApiService } from '../../../core/services/http/api.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private endpoint = 'user';

  constructor(private apiService: ApiService) {}

  checkUsernameAvailability(request: CheckUsernameAvailabilityRequestDto): Observable<CheckUsernameAvailabilityResponseDto> {
    return this.apiService.get<CheckUsernameAvailabilityResponseDto>(
      `${this.endpoint}/check-availability`,
      new HttpParams().set('username', request.username)
    );
  }

  checkEmailAvailability(request: CheckEmailAvailabilityRequestDto): Observable<CheckEmailAvailabilityResponseDto> {
    return this.apiService.get<CheckEmailAvailabilityResponseDto>(
      `${this.endpoint}/check-email-availability`,
      new HttpParams().set('email', request.email)
    );
  }

  createUser(request: CreateUserRequestDto): Observable<CreateUserResponseDto> {
    return this.apiService.post<CreateUserRequestDto, CreateUserResponseDto>(
      `${this.endpoint}/register`,
      request,
      undefined,
      { successMessage: 'User registered successfully' }
    );
  }
}