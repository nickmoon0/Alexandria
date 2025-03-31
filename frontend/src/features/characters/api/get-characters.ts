import { api } from '@/lib/api-client';
import { Character } from '@/types/app';
import { PaginatedRequest, PaginatedResponse } from '@/types/pagination';

export interface GetCharactersProps {
  pageRequest?: PaginatedRequest;
  searchString?: string;
  tagId?: string;
};

export const getCharacters = async ({ pageRequest, searchString, tagId }:GetCharactersProps) => {
  let queryString = `pageRequest=${encodeURIComponent(
    JSON.stringify(pageRequest)
  )}`;
  
  if (searchString) {
    queryString += `&searchString=${searchString}`;
  }

  if (tagId) {
    queryString += `&tagId=${tagId}`;
  }

  const response = await api.get<PaginatedResponse<Character>>(`/character?${queryString}`);
  return response.data;
};