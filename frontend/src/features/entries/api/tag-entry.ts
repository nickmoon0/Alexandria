import { api } from '@/lib/api-client';

export interface TagEntryProps {
  entryId:string;
  tagId:string;
};

export const tagEntry = async ({ entryId, tagId }:TagEntryProps) => {
  const response = await api.post(`/entry/${entryId}/tag/${tagId}`);
  return response;
};