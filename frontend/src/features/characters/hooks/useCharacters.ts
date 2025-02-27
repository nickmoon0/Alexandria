import { ToastType } from '@/components/Toast';
import { useToast } from '@/hooks/ToastContext';
import { getCharacter } from '@/features/characters/api/get-character';
import { getCharacters } from '@/features/characters/api/get-characters';
import { useState } from 'react';
import { Character } from '@/types/app';
import { useNavigate } from 'react-router';
import { paths } from '@/config/paths';
import { useCharactersContext } from '@/features/characters/hooks/CharactersContext';
import { deleteCharacter } from '@/features/characters/api/delete-character';
import { updateCharacter } from '@/features/characters/api/update-character';

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

  const fetchCharacters = async (cursorId: string | null = null, previous:boolean = false) => {
    try {
      const pageRequest = { PageSize: count, CursorId: cursorId };
      const response = await getCharacters({ pageRequest });

      const sortedData = response.data.sort((a, b) => new Date(b.createdOnUtc).getTime() - new Date(a.createdOnUtc).getTime());

      setCharacters(sortedData);
      setNextCursor(response.paging.nextCursor);

      if (!previous && cursorId !== null) {
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

  const refreshCharacters = () => {
    setCursorStack([]);
    setNextCursor(null);
    fetchCharacters();
  };

  return {
    count,
    characters,
    charactersRefresh,
    nextCursor,
    cursorStack,
    handleDelete,
    handleCharacterClick,
    fetchCharacter,
    fetchCharacters,
    setCount,
    setCursorStack,
    triggerCharactersRefresh,
    handleCharacterUpdate,
    refreshCharacters
  };
};