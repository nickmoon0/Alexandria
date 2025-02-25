import { api } from '@/lib/api-client';
import { Character } from '@/types/app';
import { PaginatedRequest, PaginatedResponse } from '@/types/pagination';

export interface GetCharactersProps {
  pageRequest?: PaginatedRequest
}

export const getCharacters = async ({ pageRequest }:GetCharactersProps) => {
  console.log(pageRequest);
  const queryString = `pageRequest=${encodeURIComponent(
    JSON.stringify(pageRequest)
  )}`;
  
  const response = await api.get<PaginatedResponse<Character>>(`/character?${queryString}`);
  return response.data;
};