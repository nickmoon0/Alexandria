import { api } from '@/lib/api-client';
import { getDocumentUrl, TokenPermissions } from '@/lib/document-service';

export interface CreateEntryProps {
  entryName:string;
  description:string;
  file:File;
};

export const createEntry = async ({ entryName, description, file }:CreateEntryProps) => {
  // Create entry
  const createEntryResponse = await api.post('/entry', {
    FileName:file.name,
    Name:entryName,
    Description:description
  });

  if (!createEntryResponse.data || !createEntryResponse.data.documentId) {
    throw new Error('Failed to create entry: Invalid response from server');
  }

  const documentId = createEntryResponse.data.documentId;

  // Get write token for entry
  const documentUrl = await getDocumentUrl({ 
    documentId,
    tokenPermission:TokenPermissions.Write 
  });

  if (!documentUrl) {
    console.log('documentUrl is null');
    return null;
  }

  // Upload file
  const formData = new FormData();
  formData.append('file', file);

  const uploadResponse = await api.post(documentUrl, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  
  if (uploadResponse.status !== 201) {
    throw new Error('File upload failed');
  }

  return { success: true, documentId };
};