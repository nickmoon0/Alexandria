import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router';
import { useCharacters } from '@/features/characters/hooks/useCharacters';
import { Character } from '@/types/app';
import MetadataTag from '@/components/MetadataTag';
import EditableField from '@/components/Input/EditableField';
import TagInput from '@/features/tags/components/TagInput';

const CharacterRoute = () => {
  const [character, setCharacter] = useState<Character | null>(null);
  const { characterId } = useParams();
  const { fetchCharacter, handleCharacterUpdate } = useCharacters();

  const getData = async () => {
    if (!characterId) return;

    const retrievedCharacter = await fetchCharacter(characterId);
    if (!retrievedCharacter) return;

    setCharacter(retrievedCharacter);
  };

  useEffect(() => {
    getData();
  }, [characterId]);

  return (
    <div className="flex items-start justify-center max-h-screen p-6">
      <div className="p-12 max-w-4xl w-full">
        {character ? (
          <>
            <EditableField
              value={character.firstName}
              onChange={(value) => handleCharacterUpdate({ ...character, firstName: value ?? '' })}
              textClassName='text-3xl font-extrabold text-gray-800' />
              {'    '}
            <EditableField
              value={character.lastName}
              onChange={(value) => handleCharacterUpdate({ ...character, lastName: value ?? '' })}
              textClassName='text-3xl font-extrabold text-gray-800' />
            
            <TagInput
              initialTags={[]} />
            
            <div className="space-y-4">
              <EditableField
                value={character.description ?? ''}
                onChange={(value) => handleCharacterUpdate({ ...character, description: value ?? '' })}
                textClassName='text-gray-700 text-lg' />
              
              {character?.user && (
                <p className="text-sm text-gray-500 italic mt-1">
                  Character created from user
                </p>
              )}
              {character.createdBy && 
                <MetadataTag
                  createdBy={character?.createdBy}
                  createdAtUtc={character?.createdOnUtc}
                  id={character?.id}/>}
            </div>
          </>
        ) : (
          <p className="text-center text-gray-500">Loading character data...</p>
        )}
      </div>
    </div>
  );
};

export default CharacterRoute;
