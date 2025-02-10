import { api } from '@/lib/api-client';

export interface GetFileProps {
  documentId: string
};

export const getFile = async ({ documentId }:GetFileProps) => {
  const response = await api.get(`/document/${documentId}`, {
    responseType: 'blob'
  });

  const url = URL.createObjectURL(response.data);
  const contentType = response.headers["content-type"];

  return ({
    mediaUrl: url,
    contentType: contentType
  });
};
