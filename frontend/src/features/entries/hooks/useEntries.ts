import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import { getEntries, GetEntriesOptions } from '@/features/entries/api/get-entries';
import { Entry } from '@/types/app';
import { paths } from '@/config/paths';
import { useEntriesRefresh } from '@/features/entries/hooks/EntriesContext';
import { deleteEntry } from '@/features/entries/api/delete-entry';
import { useToast } from '@/hooks/ToastContext';
import { ToastType } from '@/components/Toast';

export const useEntries = () => {
  const [entries, setEntries] = useState<Entry[]>([]);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [cursorStack, setCursorStack] = useState<string[]>([]);
  const [entryPopup, setEntryPopup] = useState<Entry | null>(null);
  const [newEntryPopup, setNewEntryPopup] = useState<boolean>(false);

  const { count, entriesRefresh } = useEntriesRefresh();
  const { showToast } = useToast();

  const navigate = useNavigate();

  // Fetch Entries
  const fetchEntries = useCallback(async (cursorId: string | null, previous: boolean = false) => {
    try {
      const pageRequest = { PageSize: count, CursorId: cursorId };
      const response = await getEntries({ pageRequest, options: [ GetEntriesOptions.IncludeDocument, GetEntriesOptions.IncludeTags ] });
  
      setEntries(response.data);
      setNextCursor(response.paging.nextCursor);
  
      // Don't add cursor to stack if moving backwards
      if (!previous && cursorId !== null) {
        setCursorStack((prevStack) => [...prevStack, cursorId]);
      }
    } catch (error) {
      console.error(error);
      showToast('Failed to fetch entries', ToastType.Error);
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

  const handleDelete = async (entryId:string) => {
    try {
      await deleteEntry({ entryId });

      refreshEntries();
    } catch (error) {
      console.error(error);
      showToast('Failed to delete entry', ToastType.Error);
    }
  };

  // Refresh entries when count changes
  useEffect(() => {
    refreshEntries();
  }, [refreshEntries, count, entriesRefresh]);

  return {
    entries,
    count,
    nextCursor,
    cursorStack,
    entryPopup,
    newEntryPopup,
    handleEntryClick,
    handleEntryPopupClose,
    handleDelete,
    fetchEntries,
    setCursorStack,
    setNewEntryPopup,
    setNextCursor,
    refreshEntries
  };
};
