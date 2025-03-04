import React, { useEffect, useState } from 'react';
import { useEntries } from '@/features/entries/hooks/useEntries';
import Button from '@/components/Buttons/Button';
import TagList from '@/features/tags/components/TagList';
import Table, { Column } from '@/components/Table';
import { Entry, Tag } from '@/types/app';
import DeleteButton from '@/components/Buttons/DeleteButton';
import { formatDateTime } from '@/lib/helpers';
import { useAuth } from 'react-oidc-context';
import { Roles } from '@/config/constants';

export const EntriesTable = () => {
  const [tagFilter, setTagFilter] = useState<Tag | null>(null);

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

  const auth = useAuth();
  const currentUserId = auth.user?.profile.sub ?? '';
  const roles = auth.user?.profile.roles as string[] || [];

  const handleNextPage = () => {
    if (nextCursor) fetchEntries({ cursorId: nextCursor });
  };

  const handlePreviousPage = () => {
    if (cursorStack.length > 0) {
      const prevCursor = cursorStack[cursorStack.length - 2];
      setCursorStack((prevStack) => prevStack.slice(0, -1));
      fetchEntries({cursorId: prevCursor, previous: true});
    }
  };

  const canDeleteEntry = (entry:Entry, currentUserId:string, roles:string[]) => {
    const createdByCurrentUser = entry.createdBy.id === currentUserId;
    const isAdmin = roles.includes(Roles.ADMIN);

    // User can delete their own entries, admins can delete any entries
    return createdByCurrentUser || isAdmin;
  };

  const columns: Column<Entry>[] = [
    {
      key: 'delete',
      label: 'Delete',
      render: (entry: Entry) => (
        <DeleteButton
          disabled={!canDeleteEntry(entry, currentUserId, roles)}
          onClick={(event) => {
            event.stopPropagation();
            handleDelete(entry.id);
          }} />
      ),
    },
    { key: 'name', label: 'Name' },
    { key: 'description', label: 'Description' },
    { key: 'tags', label: 'Tags', render: (entry: Entry) => <TagList tags={entry.tags} onClick={setTagFilter} /> },
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
    refreshEntries(tagFilter?.id);
  }, [count, entriesRefresh, tagFilter]);

  return (
    <div>
      {tagFilter && (
        <div className="flex flex-row mb-4 items-center gap-2">
          <Button onClick={() => setTagFilter(null)}>
            Clear Filter
          </Button>
          <p className='text-gray-700'>{tagFilter.name}</p>
        </div>
      )}
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
