import { useState } from 'react';
import { useNavigate } from 'react-router';
import { getEntries, GetEntriesOptions } from '@/features/entries/api/get-entries';
import { Character, Entry, Tag } from '@/types/app';
import { paths } from '@/config/paths';
import { useEntriesRefresh } from '@/features/entries/hooks/EntriesContext';
import { deleteEntry } from '@/features/entries/api/delete-entry';
import { useToast } from '@/hooks/ToastContext';
import { ToastType } from '@/components/Toast';
import { updateEntry } from '@/features/entries/api/update-entry';
import { tagEntry, removeTagEntry } from '@/features/entries/api/tag-entry';
import { PaginatedRequest } from '@/types/pagination';
import { addCharacterToEntry, removeCharacterFromEntry } from '@/features/entries/api/character-entry';

export interface FetchEntriesProps {
  cursorId?: string;
  previous?: boolean;
  tagId?: string;
};

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
  const fetchEntries = async ({
    cursorId,
    previous,
    tagId,
  }:FetchEntriesProps) => {
    try {
      const pageRequest:PaginatedRequest = { PageSize: count, CursorId: cursorId };
      const response = await getEntries({ 
        pageRequest, 
        options: [ 
          GetEntriesOptions.IncludeDocument,
          GetEntriesOptions.IncludeCharacters,
          GetEntriesOptions.IncludeTags
        ],
        tagId
      });
  
      setEntries(response.data);
      setNextCursor(response.paging.nextCursor);
  
      // Don't add cursor to stack if moving backwards
      if (!previous && cursorId) {
        setCursorStack((prevStack) => [...prevStack, cursorId]);
      }
    } catch (error) {
      console.error(error);
      showToast('Failed to fetch entries', ToastType.Error);
    }
  };

  // Handle Entry Click
  const handleEntryClick = (rowId: string) => {
    navigate(paths.entry.getHref(rowId));
  };


  const refreshEntries = (tagId?:string) => {
    setCursorStack([]);
    setNextCursor(null);
    fetchEntries({ tagId });
  };

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

  const handleCharacterEntry = async (entry:Entry, character:Character): Promise<boolean> => {
    try {
      await addCharacterToEntry({ entryId:entry.id, characterId:character.id });
      return true;
    } catch (error) {
      console.error(error);
      showToast('Failed to add character to entry', ToastType.Error);
      return false;
    }
  };

  const handleRemoveCharacterEntry = async (entry:Entry, character:Character): Promise<boolean> => {
    try {
      await removeCharacterFromEntry({ entryId:entry.id, characterId:character.id });
      return true;
    } catch (error) {
      console.error(error);
      showToast('Failed to remove character from entry', ToastType.Error);
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
    handleEntryUpdate,
    handleCharacterEntry,
    handleRemoveCharacterEntry
  };
};
