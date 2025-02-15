import { api } from "@/lib/api-client";

export interface DeleteEntryProps {
  entryId:string;
};

export const deleteEntry = async ({ entryId }:DeleteEntryProps) => {
  const response = await api.delete(`/entry/${entryId}`);
  
  if (!(response.status >= 200 && response.status < 300)) {
    throw new Error(`Failed to delete entry with ID ${entryId}`);
  }
};