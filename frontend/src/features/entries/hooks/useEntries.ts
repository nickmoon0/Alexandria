import { useEffect, useState } from 'react';
import { getEntries, GetEntriesOptions } from '@/features/entries/api/get-entries';
import { Entry } from '@/types/app';

export const useEntries = () => {
  const [entries, setEntries] = useState<Entry[]>([]);
  const [count, setCount] = useState<number>(25);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [cursorStack, setCursorStack] = useState<string[]>([]);
  const [entryPopup, setEntryPopup] = useState<Entry | null>(null);
  const [newEntryPopup, setNewEntryPopup] = useState<boolean>(false);

  // Fetch Entries
  const fetchEntries = async (cursorId: string | null, previous: boolean = false) => {
    const pageRequest = { PageSize: count, CursorId: cursorId };

    const response = await getEntries({ pageRequest, options: [ GetEntriesOptions.IncludeDocument, GetEntriesOptions.IncludeTags ] });

    setEntries(response.data);
    setNextCursor(response.paging.nextCursor);

    // Don't add cursor to stack if moving backwards
    if (!previous && cursorId !== null) {
      setCursorStack((prevStack) => [...prevStack, cursorId]);
    }
  };

  // Refresh entries when count changes
  useEffect(() => {
    refreshEntries();
  }, [count]);

  // Handle Entry Click
  const handleEntryClick = (rowId: string) => {
    const entry = entries.find((entry) => entry.id === rowId);
    if (!entry) return console.error('Entry not found');
    setEntryPopup(entry);
  };

  const handleEntryPopupClose = () => {
    setEntryPopup(null);
  }

  const refreshEntries = () => {
    setCursorStack([]);
    setNextCursor(null);
    fetchEntries(null);
  };

  return {
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
  };
};
