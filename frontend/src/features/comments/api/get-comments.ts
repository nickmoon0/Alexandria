import { api } from '@/lib/api-client';

export interface GetCommentsProps {
  entryId:string
};

export const getComments = async ({ entryId }:GetCommentsProps) => {
  const response = await api.get(`/entry/${entryId}/comments`);
  return response.data;
};