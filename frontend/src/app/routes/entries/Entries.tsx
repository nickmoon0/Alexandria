import React, { useState } from 'react';
import Button from '@/components/Buttons/Button';
import { EntriesTable } from '@/features/entries/components/EntriesTable';
import EntryUploadForm from '@/features/entries/components/EntryUploadForm';
import { useEntries } from '@/features/entries/hooks/useEntries';
import { Plus } from 'lucide-react';

const EntriesRoute = () => {
  const [newEntryPopup, setNewEntryPopup] = useState<boolean>(false);

  const {
    count,
    triggerEntriesRefresh,
    setCount
  } = useEntries();

  const handleItemsPerPageChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setCount(Number(e.target.value));
  };
  
  const handleUploadClick = () => {
    setNewEntryPopup(true);
  };

  const handleCloseUploadClick = () => {
    setNewEntryPopup(false);
    triggerEntriesRefresh();
  };
  
  return (
    <div className='grid grid-cols-3 gap-4'>
      <div></div>
      <div className='col-span-full container mx-auto px-4'>
        {newEntryPopup && <EntryUploadForm onClose={handleCloseUploadClick} />}

        {/* Top controls */}
        <div className='flex justify-between items-center mb-4'>
          <div className='flex items-center space-x-2'>
            <label htmlFor='itemsPerPage' className='text-sm font-medium text-gray-600'>
              Items per page:
            </label>
            <select
              id='itemsPerPage'
              className='bg-white border border-gray-300 rounded-md py-1 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500'
              onChange={handleItemsPerPageChange}
              value={count}
            >
              <option value='10'>10</option>
              <option value='25'>25</option>
              <option value='50'>50</option>
              <option value='100'>100</option>
            </select>
          </div>
          <Button onClick={handleUploadClick} className="flex items-center space-x-1">
            <Plus size={17} />
            <span>New</span>
          </Button>
        </div>
        <EntriesTable />
      </div>
      <div></div>
    </div>
  );
};

export default EntriesRoute;