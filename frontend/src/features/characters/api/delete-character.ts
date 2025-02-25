import { api } from '@/lib/api-client';

export const deleteCharacter = async (characterId:string) => {
  const response = await api.delete(`/character/${characterId}`);
  return response.data;
};