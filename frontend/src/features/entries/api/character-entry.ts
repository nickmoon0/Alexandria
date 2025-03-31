import { api } from '@/lib/api-client';

export interface AddCharacterToEntryProps {
  entryId: string;
  characterId: string;
};

export interface RemoveCharacterFromEntryProps {
  entryId: string;
  characterId: string;
};


export const addCharacterToEntry = async ({ entryId, characterId }:AddCharacterToEntryProps) => {
  const response = await api.post(`/entry/${entryId}/character/${characterId}`);
  return response;
};

export const removeCharacterFromEntry = async ({ entryId, characterId }:RemoveCharacterFromEntryProps) => {
  const response = await api.delete(`/entry/${entryId}/character/${characterId}`);
  return response;
};