import { api } from '@/lib/api-client';
import { Character } from '@/types/app';

interface UpdateCharacterContract {
  firstName?:string;
  lastName?:string;
  middleNames?:string;
  description?:string;
};

export interface UpdateCharacterProps {
  character:Character;
};

export const updateCharacter = async ({ character }:UpdateCharacterProps) => {
  const data:UpdateCharacterContract = {
    firstName: character.firstName,
    lastName: character.lastName,
    middleNames: character.middleNames,
    description: character.description
  };

  const response = await api.patch(`/character/${character.id}`, data);
  return response;
};