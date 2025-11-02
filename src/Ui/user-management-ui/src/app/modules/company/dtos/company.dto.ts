import { IndustryDto } from "../../industry/dtos/industry.dto";

export interface CompanyDto {
  id: string;
  name: string;
  industry?: IndustryDto;
}