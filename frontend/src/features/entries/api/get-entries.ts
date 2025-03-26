import { api } from '@/lib/api-client';
import { Entry } from '@/types/app';
import { PaginatedRequest, PaginatedResponse } from '@/types/pagination';

export enum GetEntriesOptions {
  None = 'None',
  IncludeTags = 'IncludeTags',
  IncludeDocument = 'IncludeDocument',
  IncludeCharacters = 'IncludeCharacters'
};

export interface GetEntriesProps {
  options?: GetEntriesOptions[];
  pageRequest?: PaginatedRequest;
  tagId?: string;
};

export const getEntries = async ({ options, pageRequest, tagId }:GetEntriesProps) => {
  let queryString = `pageRequest=${encodeURIComponent(
    JSON.stringify(pageRequest)
  )}`;

  if (options) {
    queryString += `&options=${options.join('|')}`;
  }

  if (tagId) {
    queryString += `&tagId=${tagId}`;
  }

  const response = await api.get<PaginatedResponse<Entry>>(`/entry?${queryString}`);
  return response.data;
};