import EntryPopup from '@/features/entries/components/EntriesPopup';
import { useEntries } from '@/features/entries/hooks/useEntries';
import Button from '@/components/Button';
import TagList from '@/components/TagList';
import EntryUploadForm from './EntryUploadForm';
import { Plus } from 'lucide-react';
import { CircleX } from 'lucide-react';
import { deleteEntry } from '../api/delete-entry';

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
    count,
    nextCursor,
    cursorStack,
    entryPopup,
    newEntryPopup,
    setCount,
    handleEntryClick,
    handleEntryPopupClose,
    fetchEntries,
    setCursorStack,
    setNewEntryPopup,
    setNextCursor,
    refreshEntries
   } = useEntries();
  
  const handleItemsPerPageChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setCount(Number(e.target.value));
  };

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

  const handleUploadClick = () => {
    setNewEntryPopup(true);
  };

  const handleCloseUploadClick = () => {
    setNewEntryPopup(false);
    refreshEntries();
  };

  const handleDelete = async (entryId:string) => {
    try {
      await deleteEntry({ entryId });

      refreshEntries();
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div className='col-span-full container mx-auto px-4'>
      {entryPopup && <EntryPopup entry={entryPopup} onClose={handleEntryPopupClose} />}
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