import DeleteButton from '@/components/Buttons/DeleteButton';
import Table, { Column } from '@/components/Table';
import { Character, Tag } from '@/types/app';
import React, { useEffect, useState } from 'react';
import { useCharacters } from '@/features/characters/hooks/useCharacters';
import Button from '@/components/Buttons/Button';
import { formatDateTime } from '@/lib/helpers';
import { useAuth } from 'react-oidc-context';
import { Roles } from '@/config/constants';
import TagList from '@/features/tags/components/TagList';

export const CharactersTable = () => {
  // State management/hooks
  const [tagFilter, setTagFilter] = useState<Tag | null>(null);

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
    refreshCharacters,
  } = useCharacters();

  const auth = useAuth();
  const roles = (auth.user?.profile.roles as string[]) || [];

  useEffect(() => {
    refreshCharacters(tagFilter?.id);
  }, [count, charactersRefresh, tagFilter]);

  // Functions
  const handleNextPage = () => {
    if (nextCursor) fetchCharacters({ cursorId: nextCursor, tagId: tagFilter?.id });
  };

  const handlePreviousPage = () => {
    if (cursorStack.length > 0) {
      const prevCursor = cursorStack[cursorStack.length - 2];
      setCursorStack((prevStack) => prevStack.slice(0, -1));
      fetchCharacters({ cursorId: prevCursor, previous: true, tagId: tagFilter?.id });
    }
  };

  const canDeleteCharacter = (character: Character) => {
    // Can only delete character if you are an admin AND the character is not created from a user
    return roles.includes(Roles.ADMIN) && !character.user;
  };

  // Table setup
  const columns: Column<Character>[] = [
    {
      key: 'delete',
      label: 'Delete',
      render: (character: Character) => (
        <DeleteButton
          disabled={!canDeleteCharacter(character)}
          onClick={(event) => {
            event.stopPropagation();
            handleDelete(character.id);
          }}
        />
      ),
    },
    {
      key: 'name',
      label: 'Fullname',
      render: (character: Character) => `${character.firstName} ${character.lastName}`,
    },
    { key: 'description', label: 'Description' },
    { key: 'tags', label: 'Tags', render: (character: Character) => <TagList tags={character.tags} onClick={setTagFilter} /> },
    {
      key: 'createdBy',
      label: 'Created By',
      render: (character: Character) => `${character.createdBy.firstName} ${character.createdBy.lastName}`,
    },
    {
      key: 'createdOn',
      label: 'Created On',
      render: (character: Character) => formatDateTime(character.createdOnUtc),
    },
    {
      key: 'createdFromUser',
      label: 'Created From User',
      render: (character: Character) => (character.user ? 'Yes' : 'No'),
    },
  ];

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

      <Table columns={columns} data={characters} onRowClick={(character) => handleCharacterClick(character.id)} />

      <div className="flex justify-between items-center mt-4">
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
