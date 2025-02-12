import { api } from '@/lib/api-client';
import { Entry } from '@/types/app';
import { PaginatedRequest, PaginatedResponse } from '@/types/pagination';

export enum GetEntriesOptions {
  None = 'None',
  IncludeThumbnails = 'IncludeThumbnails',
  IncludeTags = 'IncludeTags',
  IncludeDocument = 'IncludeDocument'
};

export interface GetEntriesProps {
  options?: GetEntriesOptions[];
  pageRequest?: PaginatedRequest;
};

export const getEntries = async ({ options, pageRequest }:GetEntriesProps) => {
  var queryString = `pageRequest=${encodeURIComponent(
    JSON.stringify(pageRequest)
  )}`;

  if (options) {
    queryString += `&options=${options.join('|')}`
  }

  const response = await api.get<PaginatedResponse<Entry>>(`/entry?${queryString}`);
  return response.data;
};