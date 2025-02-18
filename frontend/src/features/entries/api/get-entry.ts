import { api } from "@/lib/api-client";

export enum GetEntryOptions {
  None = 'None',
  IncludeComments = 'IncludeComments',
  IncludeTags = 'IncludeTags',
  IncludeDocument = 'IncludeDocument'
};

export interface GetEntryProps {
  entryId:string;
  options?:GetEntryOptions[];
};

export const getEntry = async ({ entryId, options }:GetEntryProps) => {
  var queryString = '';
  if (options) {
    queryString += `?options=${options.join('|')}`;
  }

  const response = await api.get(`/entry/${entryId}${queryString}`);
  return response.data;
};