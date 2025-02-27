import DeleteButton from '@/components/Buttons/DeleteButton';
import Table, { Column } from '@/components/Table';
import { Character } from '@/types/app';
import React, { useEffect } from 'react';
import { useCharacters } from '@/features/characters/hooks/useCharacters';
import Button from '@/components/Buttons/Button';
import { formatDateTime } from '@/lib/helpers';
import { useAuth } from 'react-oidc-context';
import { Roles } from '@/config/constants';

export const CharactersTable = () => {
  // State management/hooks
  const {
    characters,
    count,
    cursorStack,
    charactersRefresh,
    nextCursor,
    handleDelete,
    handleCharacterClick,
    fetchCharacters,
    setCursorStack,
    refreshCharacters
  } = useCharacters();

  const auth = useAuth();
  const roles = auth.user?.profile.roles as string[] || [];

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
        <DeleteButton 
          disabled={!roles.includes(Roles.ADMIN)} // Only admins can delete characters
          onClick={(event) => {
            event.stopPropagation();
            handleDelete(character.id);
          }}/>
      )
    },
    {
      key: 'name',
      label: 'Fullname',
      render: (character:Character) => `${character.firstName} ${character.lastName}`
    },
    { key: 'description', label: 'Description' },
    {
      key: 'createdBy',
      label: 'Created By',
      render: (character:Character) => `${character.createdBy.firstName} ${character.createdBy.lastName}`
    },
    {
      key: 'createdOn',
      label: 'Created On',
      render: (character:Character) => formatDateTime(character.createdOnUtc)
    },
    {
      key: 'createdFromUser',
      label: 'Created From User',
      render: (character:Character) => character.user ? 'Yes' : 'No'
    }
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