import { api } from '@/lib/api-client';

export interface TagCharacterProps {
  characterId:string;
  tagId:string;
};

export const tagCharacter = async ({ characterId, tagId }:TagCharacterProps) => {
  const response = await api.post(`/character/${characterId}/tag/${tagId}`);
  return response;
};

export const removeTagCharacter = async ({ characterId, tagId }:TagCharacterProps) => {
  const response = await api.delete(`/character/${characterId}/tag/${tagId}`);
  return response;
};