export interface IPageList<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface IPageRequest {
  pageNumber?: number;
  pageSize?: number;
  filters?: string;
  sortOrder?: string;
}