import { api } from '@/lib/api-client';
import { Tag } from '@/types/app';

export interface GetTagsProps {
  searchString?: string;
  maxCount?: number;
}

export const getTags = async ({ searchString, maxCount }: GetTagsProps): Promise<Tag[]> => {
  // Build a query parameter object with strictly typed keys/values.
  const params: { searchString?: string; maxCount?: string } = {};

  if (searchString !== undefined) {
    params.searchString = searchString;
  }
  if (maxCount !== undefined) {
    // Convert maxCount from a number to a string.
    params.maxCount = maxCount.toString();
  }

  const response = await api.get<Tag[]>('/tag', { params });
  return response.data;
};
