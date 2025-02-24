import React from 'react';
import { useEntries } from '@/features/entries/hooks/useEntries';
import Button from '@/components/Button';
import TagList from '@/features/tags/components/TagList';
import { CircleX } from 'lucide-react';
import Table, { Column } from '@/components/Table';
import { Entry } from '@/types/app';

interface DeleteButtonProps {
  onClick: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

const DeleteButton = ({ onClick }: DeleteButtonProps) => {
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
    if (nextCursor) fetchEntries(nextCursor);
  };

  const handlePreviousPage = () => {
    if (cursorStack.length > 0) {
      const prevCursor = cursorStack[cursorStack.length - 2];
      setCursorStack((prevStack) => prevStack.slice(0, -1));
      fetchEntries(prevCursor, true);
    }
  };

  const columns: Column<Entry>[] = [
    {
      key: 'delete',
      label: 'Delete',
      render: (entry: Entry) => (
        <DeleteButton onClick={(event) => {
          event.stopPropagation();
          handleDelete(entry.id);
        }} />
      ),
    },
    { key: 'name', label: 'Name' },
    { key: 'description', label: 'Description' },
    { key: 'tags', label: 'Tags', render: (entry: Entry) => <TagList tags={entry.tags} /> },
  ];

  return (
    <div>
      <Table columns={columns} data={entries} onRowClick={(entry) => handleEntryClick(entry.id)} />

      <div className='flex justify-between items-center mt-4'>
        <Button onClick={handlePreviousPage} disabled={cursorStack.length <= 0}>
          Previous Page
        </Button>
        <Button onClick={handleNextPage} disabled={!nextCursor}>
          Next Page
        </Button>
      </div>
    </div>
  );
};
