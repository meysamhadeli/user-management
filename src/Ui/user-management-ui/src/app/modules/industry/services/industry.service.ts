import { IPageList } from '@/core/models/pagination';
import { ApiService } from '@/core/services/http/api.service';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { IndustryDto } from '@/modules/industry/dtos/industry.dto';
import { CreateIndustryRequestDto } from '@/modules/industry/dtos/create-industry-request.dto';
import { CreateIndustryResponseDto } from '@/modules/industry/dtos/create-industry-response.dto';

@Injectable({
  providedIn: 'root'
})
export class IndustryService {
  private endpoint = 'industry';

  constructor(private apiService: ApiService) {}

  getIndustries(): Observable<IPageList<IndustryDto>> {
    return this.apiService.get<IPageList<IndustryDto>>(this.endpoint);
  }

  createIndustry(request: CreateIndustryRequestDto): Observable<CreateIndustryResponseDto> {
    return this.apiService.post<CreateIndustryRequestDto, CreateIndustryResponseDto>(
      this.endpoint, 
      request,
      undefined,
      { successMessage: 'Industry created successfully' }
    );
  }
}