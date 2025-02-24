import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router';
import { useCharacters } from '@/features/characters/hooks/useCharacters';
import { Character } from '@/types/app';
import MetadataTag from '@/components/MetadataTag';

const CharacterRoute = () => {
  const [character, setCharacter] = useState<Character | null>(null);
  const { characterId } = useParams();
  const { retrieveCharacter } = useCharacters();

  const getData = async () => {
    if (!characterId) return;

    const retrievedCharacter = await retrieveCharacter(characterId);
    if (!retrievedCharacter) return;

    setCharacter(retrievedCharacter);
  };

  useEffect(() => {
    getData();
  }, [characterId]);

  return (
    <div className="flex items-start justify-center min-h-screen p-6">
      <div className="p-12 max-w-4xl w-full">
        {character ? (
          <>
            <h1 className="text-3xl font-extrabold text-gray-800">
              {character.firstName} {character.lastName}
            </h1>
            <div className="space-y-4 text-gray-700 text-lg">
              <p>{character.description}</p>
              
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
