import DeleteButton from '@/components/Buttons/DeleteButton';
import Table, { Column } from '@/components/Table';
import { Character } from '@/types/app';
import React, { useEffect } from 'react';
import { useCharacters } from '@/features/characters/hooks/useCharacters';
import Button from '@/components/Buttons/Button';

export const CharactersTable = () => {
  // State management/hooks
  const {
    characters,
    count,
    cursorStack,
    charactersRefresh,
    nextCursor,
    deleteCharacter,
    handleCharacterClick,
    fetchCharacters,
    setCursorStack,
    refreshCharacters
  } = useCharacters();

  useEffect(() => {
    refreshCharacters();
  }, [count, charactersRefresh]);

  // Functions

  const handleNextPage = () => {
    if (nextCursor) fetchCharacters(nextCursor);
  };

  const handlePreviousPage = () => {
    if (cursorStack.length > 0) {
      const prevCursor = cursorStack[cursorStack.length - 2];
      setCursorStack((prevStack) => prevStack.slice(0, -1));
      fetchCharacters(prevCursor, true);
    }
  };

  // Table setup
  const columns: Column<Character>[] = [
    {
      key: 'delete',
      label: 'Delete',
      render: (character:Character) => (
        <DeleteButton onClick={(event) => {
          event.stopPropagation();
          deleteCharacter(character.id);
        }} />
      )
    },
    { 
      key: 'name',
      label: 'Fullname',
      render: (character:Character) => `${character.firstName} ${character.lastName}`
    },
    { key: 'description', label: 'Description' }
  ];

  return (
    <div>
      <Table columns={columns} data={characters} onRowClick={(character) => handleCharacterClick(character.id)}/>

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