import { api } from '@/lib/api-client';

export interface CreateTagProps {
  name:string;
};

export const createTag = async ({ name }:CreateTagProps) => {
  const response = await api.post('/tag', { name });
  return response.data;
};