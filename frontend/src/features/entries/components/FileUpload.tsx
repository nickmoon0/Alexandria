import { Trash2 } from 'lucide-react';

interface FileUplodProps {
  selectedFile?: File;
  handleFileChange: (event:React.ChangeEvent<HTMLInputElement>) => void;
  handleClearFile: () => void;
  handleUpload: () => void;
};

const FileUpload = ({ selectedFile, handleFileChange, handleClearFile, handleUpload }:FileUplodProps) => {
  return (
    <div className='flex flex-col items-start space-y-1'>
      {selectedFile && (
        <span className='text-gray-400 text-sm'>{selectedFile.name}</span>
      )}
      <input
        type='file'
        onChange={handleFileChange}
        className='hidden'
        id='fileInput'
      />
      <div className='flex space-x-2'>
        <button
          onClick={handleUpload}
          className='px-4 py-2 bg-blue-600 text-white font-medium rounded-lg shadow-md cursor-pointer hover:bg-blue-700 transition'
        >
          {selectedFile ? 'Upload' : 'Select File'}
        </button>
        {selectedFile && (
          <button
            onClick={handleClearFile}
            className='px-4 py-2 bg-gray-600 text-white font-medium rounded-lg shadow-md cursor-pointer hover:bg-gray-700 transition'>
            <Trash2 size={20} />
          </button>
        )}
      </div>
    </div>
  );
};

export default FileUpload;