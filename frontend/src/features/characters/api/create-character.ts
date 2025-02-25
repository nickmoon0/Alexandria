import { api } from '@/lib/api-client';

export interface CreateCharacterProps {
  firstName:string;
  lastName:string;
  middleNames?:string | null;
  description?:string | null;
};

export const createCharacter = async ({
  firstName,
  lastName,
  middleNames = null,
  description = null
}:CreateCharacterProps) => {
  const response = await api.post('/character', {
    firstName,
    lastName,
    middleNames,
    description
  });

  return response.data;
};