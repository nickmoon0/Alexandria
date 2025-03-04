import { ToastType } from '@/components/Toast';
import { useToast } from '@/hooks/ToastContext';
import { getCharacter } from '@/features/characters/api/get-character';
import { getCharacters } from '@/features/characters/api/get-characters';
import { useState } from 'react';
import { Character, Tag } from '@/types/app';
import { useNavigate } from 'react-router';
import { paths } from '@/config/paths';
import { useCharactersContext } from '@/features/characters/hooks/CharactersContext';
import { deleteCharacter } from '@/features/characters/api/delete-character';
import { updateCharacter } from '@/features/characters/api/update-character';
import { removeTagCharacter, tagCharacter } from '@/features/characters/api/tag-character';
import { PaginatedRequest } from '@/types/pagination';

export interface FetchCharactersProps {
  cursorId?: string;
  previous?: boolean;
  tagId?: string;
};

export const useCharacters = () => {
  const [characters, setCharacters] = useState<Character[]>([]);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [cursorStack, setCursorStack] = useState<string[]>([]);

  const {
    charactersRefresh,
    count,
    setCount,
    triggerCharactersRefresh 
  } = useCharactersContext();
  const { showToast } = useToast();
  const navigate = useNavigate();

  const handleCharacterClick = (rowId:string) => {
    navigate(paths.character.getHref(rowId));
  };

  const fetchCharacter = async (characterId:string) => {
    try {
      const character = await getCharacter({ characterId });
      return character;
    } catch (error) {
      console.error(error);
      showToast('Failed to retrieve character', ToastType.Error);
    }
  };

  const fetchCharacters = async ({ cursorId, previous, tagId }:FetchCharactersProps) => {
    try {
      const pageRequest:PaginatedRequest = { PageSize: count, CursorId: cursorId };
      const response = await getCharacters({ pageRequest, tagId });

      const sortedData = response.data.sort((a, b) => new Date(b.createdOnUtc).getTime() - new Date(a.createdOnUtc).getTime());

      setCharacters(sortedData);
      setNextCursor(response.paging.nextCursor);

      if (!previous && cursorId) {
        setCursorStack((prevStack) => [...prevStack, cursorId]);
      }
    } catch (error) {
      console.error(error);
      showToast('Failed to retrieve characters', ToastType.Error);
    }
  };

  const handleDelete = async (characterId:string) => {
    try {
      const response = await deleteCharacter(characterId);
      refreshCharacters();
      return response;
    } catch (error) {
      console.error(error);
      showToast('Failed to delete character', ToastType.Error);
    }
  };

  const handleCharacterUpdate = async (character:Character) => {
    try {
      await updateCharacter({ character });
    } catch (error) {
      console.error(error);
      showToast('Failed to update character', ToastType.Error);
    }
  };

  const handleTagCharacter = async (character:Character, tag:Tag) => {
    try {
      await tagCharacter({ characterId:character.id, tagId:tag.id });
      return true;
    } catch (error) {
      console.error(error);
      showToast('Failed to tag character', ToastType.Error);
      return false;
    }
  };

  const handleRemoveTagCharacter = async (character:Character, tag:Tag) => {
    try {
      await removeTagCharacter({ characterId:character.id, tagId:tag.id });
      return true;
    } catch (error) {
      console.error(error);
      showToast('Failed to remove tag on character', ToastType.Error);
      return false;
    }
  };

  const refreshCharacters = (tagId:string | undefined = undefined) => {
    setCursorStack([]);
    setNextCursor(null);
    fetchCharacters({ tagId });
  };

  return {
    count,
    characters,
    charactersRefresh,
    nextCursor,
    cursorStack,
    handleDelete,
    handleCharacterClick,
    handleTagCharacter,
    handleRemoveTagCharacter,
    fetchCharacter,
    fetchCharacters,
    setCount,
    setCursorStack,
    triggerCharactersRefresh,
    handleCharacterUpdate,
    refreshCharacters
  };
};