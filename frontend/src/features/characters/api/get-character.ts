import { api } from '@/lib/api-client';

export interface GetCharacterProps {
  characterId:string;
};

export const getCharacter = async ({ characterId }:GetCharacterProps) => {
  const response = await api.get(`/character/${characterId}`);
  return response.data;
};