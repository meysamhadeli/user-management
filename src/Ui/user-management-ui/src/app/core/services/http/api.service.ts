import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { APP_CONFIG, AppConfig } from '@/core/models/config';
import { NotificationService } from '@/core/services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  protected readonly apiAddress: string;

  constructor(
    private http: HttpClient,
    private notificationService: NotificationService,
    @Inject(APP_CONFIG) config: AppConfig
  ) {
    this.apiAddress = `${config.apiHost}${config.apiEndpoint ? `/${config.apiEndpoint}` : ''}`;
  }

  public get<T>(
    route: string,
    params?: HttpParams,
    descriptor?: OperationDescriptor
  ): Observable<T> {
    return this.http.get<T>(`${this.apiAddress}/${route}`, {
      params,
      headers: this.getHeaders(descriptor)
    }).pipe(
      this.handleResponse(descriptor)
    );
  }

  public post<TSend, TResult>(
    route: string,
    payload: TSend,
    params?: HttpParams,
    descriptor?: OperationDescriptor
  ): Observable<TResult> {
    return this.http.post<TResult>(`${this.apiAddress}/${route}`, payload, {
      params,
      headers: this.getHeaders(descriptor)
    }).pipe(
      this.handleResponse(descriptor)
    );
  }

  public put<TSend, TResult>(
    route: string,
    payload: TSend,
    params?: HttpParams,
    descriptor?: OperationDescriptor
  ): Observable<TResult> {
    return this.http.put<TResult>(`${this.apiAddress}/${route}`, payload, {
      params,
      headers: this.getHeaders(descriptor)
    }).pipe(
      this.handleResponse(descriptor)
    );
  }

  public delete<TResult>(
    route: string,
    params?: HttpParams,
    descriptor?: OperationDescriptor
  ): Observable<TResult> {
    return this.http.delete<TResult>(`${this.apiAddress}/${route}`, {
      params,
      headers: this.getHeaders(descriptor)
    }).pipe(
      this.handleResponse(descriptor)
    );
  }

  private getHeaders(descriptor?: OperationDescriptor): HttpHeaders {
    let headers = new HttpHeaders();

    if (descriptor?.cacheResponse) {
      headers = headers.set('Client-Cached', 'true');
    }

    if (descriptor?.forceRefresh) {
      headers = headers.set('Force-Refresh', 'true');
    }

    return headers;
  }

  private handleResponse<T>(descriptor?: OperationDescriptor) {
    return (source: Observable<T>): Observable<T> =>
      source.pipe(
        tap(() => {
          if (descriptor?.successMessage) {
            this.notificationService.showSuccess(descriptor.successMessage);
          }
        }),
        catchError((error: any) => {
          // Handle error messages
          if (descriptor?.failMessage) {
            this.notificationService.showError(descriptor.failMessage);
          } else if (error.error?.detail) {
            // Use ProblemDetails from backend
            this.notificationService.showError(error.error.detail);
          } else if (error.error?.title) {
            this.notificationService.showError(error.error.title);
          } else if (error.message) {
            this.notificationService.showError(error.message);
          } else {
            this.notificationService.showError('An unexpected error occurred');
          }

          return throwError(() => error);
        })
      );
  }
}

export interface OperationDescriptor {
  failMessage?: string;
  successMessage?: string;
  cacheResponse?: boolean;
  forceRefresh?: boolean;
}