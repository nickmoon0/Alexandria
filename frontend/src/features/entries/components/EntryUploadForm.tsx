import FileUpload from '@/features/entries/components/FileUpload';
import TextArea from '@/components/Input/TextArea';
import TextInput from '@/components/Input/TextInput';
import Popup from '@/components/Popup';
import { useState } from 'react';
import { createEntry } from '../api/create-entry';

export interface EntryUploadFormProps {
  onClose: () => void
};

const EntryUploadForm = ({ onClose }: EntryUploadFormProps) => {
  const [nameValue, setNameValue] = useState<string | null>(null);
  const [descValue, setDescValue] = useState<string | null>(null);
  const [fileValue, setFileValue] = useState<File | undefined>(undefined);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files.length > 0) {
      setFileValue(event.target.files[0]);
    }
  };

  const handleClearFile = () => {
    setFileValue(undefined);
  };

  const handleUpload = async () => {
    if (!fileValue) {
      document.getElementById('fileInput')?.click();
      return;
    }

    const formData = new FormData();
    formData.append('file', fileValue);

    try {
      await createEntry({ 
        entryName:nameValue ?? '',
        description:descValue ?? '',
        file:fileValue });

      onClose();
    } catch (error) {
      console.error('Upload error:', error);
      alert('Error uploading file!');
    }
  };

  const FileUploadControl = (
    <FileUpload
      selectedFile={fileValue}
      handleFileChange={handleFileChange}
      handleClearFile={handleClearFile}
      handleUpload={handleUpload} />
  );

  return (
    <div>
      <Popup title={'New Entry'} onClose={onClose} controlChildren={FileUploadControl}>
        <div className='grid grid-cols-1 gap-4 pt-2'>
          <TextInput value={nameValue ?? ''} onChange={setNameValue} placeholder='Entry Name' className='w-full' />
          <TextArea value={descValue ?? ''} onChange={setDescValue} placeholder='Entry Description' className='w-full' />
        </div>
      </Popup>
    </div>
  );
};

export default EntryUploadForm;
