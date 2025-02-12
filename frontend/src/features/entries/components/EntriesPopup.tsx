import Button from '@/components/Button';
import { Entry } from '@/types/app';
import React, { useState } from 'react';
import MediaViewer from '@/components/MediaViewer';
import TagList from '@/components/TagList';

export interface EntryPopupProps {
  entry: Entry;
  onClose: () => void; // Function to close popup
}

const EntryPopup: React.FC<EntryPopupProps> = ({ entry, onClose }) => {
  const [showDetails, setShowDetails] = useState<boolean>(false);

  if (!entry) return null; // Prevent rendering if no entry

  return (
    <div className='fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50'>
      {/* Popup Card */}
      <div className='bg-white p-6 rounded-2xl shadow-2xl max-w-lg w-full relative animate-fadeIn'>
        {/* Close Button */}
        <button
          className='absolute top-4 right-4 text-gray-500 hover:text-gray-800 text-lg'
          onClick={onClose}
        >
          ✖
        </button>

        {/* Content */}
        <h1 className='text-2xl font-bold text-gray-800 mb-3'>{entry.name}</h1>
        { entry.tags && <TagList tags={entry.tags} tagListClassName='mb-2' /> }
        <p className='text-gray-600 pb-3'>{entry.description}</p>
        <MediaViewer documentId={entry.document.id} />
        { showDetails && 
          <div>
            <p className='text-gray-600 mt-4 mb-2'><span className='font-medium'>Document ID: </span>{entry.id}</p>
            <p className='text-gray-600 mb-2'><span className='font-medium'>Created By: </span>{entry.createdBy.firstName} {entry.createdBy.lastName}</p>
            <p className='text-gray-600 mb-2'><span className='font-medium'>Created At: </span>{entry.createdAtUtc.toString()}</p>
          </div>
        }
        <div className='flex justify-between pt-4'>
          <Button onClick={() => setShowDetails(!showDetails)} className='mr-2'>
            { showDetails ? 'Hide Details' : 'Show Details' }
          </Button>
          <Button onClick={onClose}>
            Close
          </Button>
        </div>
      </div>
    </div>
  );
};

export default EntryPopup;
