import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateCompanyRequestDto } from '../dtos/create-company-request.dto';
import { CreateCompanyResponseDto } from '../dtos/create-company-response.dto';
import { CompanyDto } from '../dtos/company.dto';
import { ApiService } from '../../../core/services/http/api.service';
import { IPageList } from '../../../core/models/pagination';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  constructor(private apiService: ApiService) {}

  createCompany(request: CreateCompanyRequestDto): Observable<CreateCompanyResponseDto> {
    return this.apiService.post<CreateCompanyRequestDto, CreateCompanyResponseDto>(
      'company', 
      request,
      undefined,
      { 
        successMessage: 'Company created successfully',
        failMessage: 'Failed to create company'
      }
    );
  }

  getCompanies(
    pageNumber: number = 1, 
    pageSize: number = 20, 
    filters?: string | null, 
    sortOrder?: string | null
  ): Observable<IPageList<CompanyDto>> {
    let params: any = {
      PageNumber: pageNumber.toString(),
      PageSize: pageSize.toString()
    };

    if (filters) {
      params.Filters = filters;
    }

    if (sortOrder) {
      params.SortOrder = sortOrder;
    }

    return this.apiService.get<IPageList<CompanyDto>>('companies', params);
  }

  getCompanyById(id: string): Observable<CompanyDto> {
    return this.apiService.get<CompanyDto>(`companies/${id}`);
  }
}