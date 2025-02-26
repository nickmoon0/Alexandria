import { api } from '@/lib/api-client';
import { Entry } from '@/types/app';

interface UpdateEntryContract {
  name?:string,
  description?:string
};

export interface UpdateEntryProps {
  entry:Entry;
}

export const updateEntry = async ({ entry }:UpdateEntryProps) => {
  const data:UpdateEntryContract = {
    name: entry.name,
    description: entry.description
  };

  const response = await api.patch(`/entry/${entry.id}`, data);
  return response;
};