import React, { useEffect } from 'react';
import { useEntries } from '@/features/entries/hooks/useEntries';
import Button from '@/components/Buttons/Button';
import TagList from '@/features/tags/components/TagList';
import Table, { Column } from '@/components/Table';
import { Entry } from '@/types/app';
import DeleteButton from '@/components/Buttons/DeleteButton';
import { formatDateTime } from '@/lib/helpers';

export const EntriesTable = () => {
  const {
    count,
    entries,
    entriesRefresh,
    nextCursor,
    cursorStack,
    handleEntryClick,
    fetchEntries,
    setCursorStack,
    handleDelete,
    refreshEntries
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
    {
      key: 'createdBy',
      label: 'Created By',
      render: (entry:Entry) => `${entry.createdBy.firstName} ${entry.createdBy.lastName}`
    },
    {
      key: 'createdOn',
      label: 'Created On',
      render: (entry:Entry) => formatDateTime(entry.createdAtUtc)
    }
  ];

  // Refresh entries when count changes
  useEffect(() => {
    refreshEntries();
  }, [refreshEntries, count, entriesRefresh]);

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
