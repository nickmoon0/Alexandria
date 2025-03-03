import { useCallback, useState } from 'react';
import { useNavigate } from 'react-router';
import { getEntries, GetEntriesOptions } from '@/features/entries/api/get-entries';
import { Entry, Tag } from '@/types/app';
import { paths } from '@/config/paths';
import { useEntriesRefresh } from '@/features/entries/hooks/EntriesContext';
import { deleteEntry } from '@/features/entries/api/delete-entry';
import { useToast } from '@/hooks/ToastContext';
import { ToastType } from '@/components/Toast';
import { updateEntry } from '@/features/entries/api/update-entry';
import { tagEntry, removeTagEntry } from '@/features/entries/api/tag-entry';

export const useEntries = () => {
  const [entries, setEntries] = useState<Entry[]>([]);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [cursorStack, setCursorStack] = useState<string[]>([]);

  const { 
    count,
    entriesRefresh,
    setCount,
    triggerEntriesRefresh 
  } = useEntriesRefresh();
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

  const handleEntryUpdate = async (entry:Entry) => {
    try {
      await updateEntry({ entry });
    } catch (error) {
      console.error(error);
      showToast('Failed to update entry', ToastType.Error);
    }
  };

  const handleTagEntry = async (entry:Entry, tag:Tag): Promise<boolean> => {
    try {
      await tagEntry({ entryId:entry.id, tagId:tag.id });
      return true;
    } catch (error) {
      console.error(error);
      showToast('Failed to tag entry', ToastType.Error);
      return false;
    }
  };

  const handleRemoveTagEntry = async (entry:Entry, tag:Tag): Promise<boolean> => {
    try {
      await removeTagEntry({ entryId:entry.id, tagId:tag.id });
      return true;
    } catch (error) {
      console.error(error);
      showToast('Failed to remove tag from entry', ToastType.Error);
      return false;
    }
  };

  return {
    entries,
    entriesRefresh,
    count,
    nextCursor,
    cursorStack,
    handleEntryClick,
    handleDelete,
    handleTagEntry,
    handleRemoveTagEntry,
    fetchEntries,
    setCount,
    setCursorStack,
    setNextCursor,
    refreshEntries,
    triggerEntriesRefresh,
    handleEntryUpdate
  };
};
