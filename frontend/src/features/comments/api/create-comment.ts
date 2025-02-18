import { api } from "@/lib/api-client";

export interface AddCommentProps {
  entryId:string;
  content:string;
};

export const createComment = async ({ content, entryId }:AddCommentProps) => {
  const response = await api.post(`/entry/${entryId}/comments`, {
    content
  });

  return response.data;
};