import Button from '@/components/Buttons/Button';
import { CharactersTable } from '@/features/characters/components/CharactersTable';
import { useCharacters } from '@/features/characters/hooks/useCharacters';
import { Plus } from 'lucide-react';
import React, { useState } from 'react';

const CharactersRoute = () => {
  const [newCharacterPopup, setNewCharacterPopup] = useState<boolean>(false);

  const {
    count,
    triggerCharactersRefresh,
    setCount
  } = useCharacters();

  const handleItemsPerPageChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setCount(Number(e.target.value));
  };

  const handleUploadClick = () => {
    setNewCharacterPopup(true);
  };

  const handleCloseUploadClick = () => {
    setNewCharacterPopup(false);
    triggerCharactersRefresh();
  };

  return (
    <div className='grid grid-cols-3 gap-4'>
      <div></div>
      <div className='col-span-full container mx-auto px-4'>
        {newCharacterPopup && <Button onClick={handleCloseUploadClick}>Click</Button>}
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
              <option value='25'>25</option>
              <option value='50'>50</option>
              <option value='75'>75</option>
              <option value='100'>100</option>
            </select>
          </div>
          <Button onClick={handleUploadClick} className="flex items-center space-x-1">
            <Plus size={17} />
            <span>New</span>
          </Button>
        </div>
        <CharactersTable />
      </div>
      <div></div>
    </div>
  );
};

export default CharactersRoute;