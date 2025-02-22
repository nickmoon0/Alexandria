import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import { getEntries, GetEntriesOptions } from '@/features/entries/api/get-entries';
import { Entry } from '@/types/app';
import { paths } from '@/config/paths';

export const useEntries = () => {
  const [entries, setEntries] = useState<Entry[]>([]);
  const [count, setCount] = useState<number>(25);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [cursorStack, setCursorStack] = useState<string[]>([]);
  const [entryPopup, setEntryPopup] = useState<Entry | null>(null);
  const [newEntryPopup, setNewEntryPopup] = useState<boolean>(false);

  const navigate = useNavigate();

  // Fetch Entries
  const fetchEntries = useCallback(async (cursorId: string | null, previous: boolean = false) => {
    const pageRequest = { PageSize: count, CursorId: cursorId };

    const response = await getEntries({ pageRequest, options: [ GetEntriesOptions.IncludeDocument, GetEntriesOptions.IncludeTags ] });

    setEntries(response.data);
    setNextCursor(response.paging.nextCursor);

    // Don't add cursor to stack if moving backwards
    if (!previous && cursorId !== null) {
      setCursorStack((prevStack) => [...prevStack, cursorId]);
    }
  }, [count]);

  // Handle Entry Click
  const handleEntryClick = (rowId: string) => {
    navigate(paths.entry.getHref(rowId));
  };

  const handleEntryPopupClose = () => {
    setEntryPopup(null);
  };

  const refreshEntries = useCallback(() => {
    setCursorStack([]);
    setNextCursor(null);
    fetchEntries(null);
  }, [fetchEntries]);

  // Refresh entries when count changes
  useEffect(() => {
    refreshEntries();
  }, [refreshEntries, count]);

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
