import React from 'react';
import { useEntries } from '@/features/entries/hooks/useEntries';
import Button from '@/components/Button';
import TagList from '@/features/tags/components/TagList';
import { CircleX } from 'lucide-react';

interface DeleteButtonProps {
  onClick: (event:React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
};

const DeleteButton = ({ onClick }:DeleteButtonProps) => {
  return (
    <button onClick={onClick} className='px-2 py-2 text-red-600 font-medium rounded-lg shadow-md hover:bg-red-700 hover:text-white transition'>
      <CircleX />
    </button>
  );
};

export const EntriesTable = () => {
  const {
    entries,
    nextCursor,
    cursorStack,
    handleEntryClick,
    fetchEntries,
    setCursorStack,
    handleDelete
   } = useEntries();
  

  const handleNextPage = () => {
    if (nextCursor) {
      fetchEntries(nextCursor);
    }
  };

  const handlePreviousPage = () => {
    if (cursorStack.length > 0) {
      // Get the second last cursor for going back
      const prevCursor = cursorStack[cursorStack.length - 2];
      setCursorStack((prevStack) => prevStack.slice(0, -1));
      fetchEntries(prevCursor, true);
    }
  };

  return (
    <div>
      {/* Table */}
      <div className='overflow-x-auto rounded-lg shadow-lg'>
        <table className='min-w-full bg-white border border-gray-200'>
          <thead className='bg-gray-50 border-b'>
            <tr>
              <th className='py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider'>Delete</th>
              <th className='py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider'>Name</th>
              <th className='py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider'>Description</th>
              <th className='py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider'>Tags</th>
            </tr>
          </thead>
          <tbody className='divide-y divide-gray-200'>
            {entries.map(entry => (
              <tr onClick={() => handleEntryClick(entry.id)} className='hover:bg-gray-50' key={entry.id}>
                <td className='py-4 px-6 text-gray-600'><DeleteButton onClick={(event) => {
                  event.stopPropagation();
                  handleDelete(entry.id);
                }} /></td>
                <td className='py-4 px-6 text-gray-600'>{entry.name}</td>
                <td className='py-4 px-6 text-gray-600'>{entry.description}</td>
                <td className='py-4 px-6 text-gray-600'><TagList tags={entry.tags} /></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Pagination controls */}
      <div className='flex justify-between items-center mt-4'>
        <Button
          onClick={handlePreviousPage}
          disabled={cursorStack.length <= 0}>
          Previous Page
        </Button>
        <Button
          onClick={handleNextPage}
          disabled={!nextCursor}>
          Next Page
        </Button>
      </div>
    </div>
  );
};