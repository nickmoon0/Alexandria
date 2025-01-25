export interface PagingState {
  nextCursor:string | null;
  previousCursor:string | null;
};

export interface PaginatedRequest {
  CursorId: string | null;
  PageSize: number;
};

export interface PaginatedResponse<T> {
  data: T[];
  paging: PagingState;
};