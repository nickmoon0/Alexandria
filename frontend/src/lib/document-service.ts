import { env } from '@/config/env';
import { api } from '@/lib/api-client';

interface GetDocumentParamsProps {
  documentId: string;
};

interface DocumentParams {
  url: string;
  contentType: string;
};

export const getDocumentParams = async ({ documentId }: GetDocumentParamsProps): Promise<DocumentParams | null> => {
  try {
    // Get token
    const tokenResponse = await api.get(`/document/${documentId}/token/Read`);

    if (!tokenResponse.data.token) {
      console.error('Failed to get document token');
      return null;
    }

    const token = encodeURIComponent(tokenResponse.data.token);
    const documentUrl = `${env.FILE_API_URL}/document?token=${token}`;

    // Get content type
    const contentTypeResponse = await api.head(documentUrl);

    if (!contentTypeResponse.headers['content-type']) {
      console.error('Failed to retrieve content type');
      return null;
    }

    const contentType = contentTypeResponse.headers['content-type'];

    return { url: documentUrl, contentType };
  } catch (error) {
    console.error('Error fetching document parameters:', error);
    return null;
  }
};