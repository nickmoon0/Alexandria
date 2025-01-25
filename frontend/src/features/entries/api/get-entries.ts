import { api } from '@/lib/api-client';
import { Entry } from '@/types/app';
import { PaginatedRequest, PaginatedResponse } from '@/types/pagination';

export enum GetEntriesOptions {
  None = 'None',
  IncludeThumbnails = 'IncludeThumbnails',
  IncludeTags = 'IncludeTags',
};

export interface GetEntriesParams {
  options?: GetEntriesOptions;
  pageRequest?: PaginatedRequest;
};

export const getEntries = async (params:GetEntriesParams) => {
  const queryString = `pageRequest=${encodeURIComponent(
    JSON.stringify(params.pageRequest)
  )}`;

  const response = await api.get<PaginatedResponse<Entry>>(`/entry?${queryString}`);
  return response.data;
};