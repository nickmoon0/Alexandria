import Button from '@/components/Button';
import { Entry } from '@/types/app';
import React, { useState } from 'react';
import MediaViewer from '@/components/MediaViewer';
import TagList from '@/components/TagList';
import Popup from '@/components/Popup';

export interface EntryPopupProps {
  entry: Entry;
  onClose: () => void; // Function to close popup
}

const EntryPopup: React.FC<EntryPopupProps> = ({ entry, onClose }) => {
  const [showDetails, setShowDetails] = useState<boolean>(false);

  if (!entry) return null; // Prevent rendering if no entry

  const Control = (
    <Button onClick={() => setShowDetails(!showDetails)} className='mr-2'>
      {showDetails ? 'Hide Details' : 'Show Details'}
    </Button>
  );

  return (
    <Popup title={entry.name} onClose={onClose} controlChildren={Control} >
      { entry.tags && <TagList tags={entry.tags} /> }
      <div className='pt-2'>
        {/* Content */}
        <p className='text-gray-600 pb-3'>{entry.description}</p>
        <MediaViewer documentId={entry.document.id} />
        { showDetails && 
          <div>
            <p className='text-gray-600 mt-4 mb-2'><span className='font-medium'>Entry ID: </span>{entry.id}</p>
            <p className='text-gray-600 mb-2'><span className='font-medium'>Created By: </span>{entry.createdBy.firstName} {entry.createdBy.lastName}</p>
            <p className='text-gray-600 mb-2'><span className='font-medium'>Created At: </span>{entry.createdAtUtc.toString()}</p>
          </div>
        }
      </div>
    </Popup>
  );
};

export default EntryPopup;
