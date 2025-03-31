import React from 'react';
import { Character } from '@/types/app';

export interface CharacterListProps {
  characters: Character[];
  listClassName?: string;
  characterClassName?: string;
  onClick?: (character: Character) => void;
}

export interface ClickableCharacterProps {
  character: Character;
  className?: string;
  onClick?: (e: React.MouseEvent<HTMLDivElement>) => void;
}

const ClickableCharacter = ({ character, className, onClick }: ClickableCharacterProps) => {
  const fullName = `${character.firstName} ${character.middleNames ? character.middleNames + ' ' : ''}${character.lastName}`;

  return (
    <span
      onClick={onClick}
      className={`px-3 py-1 cursor-pointer text-sm font-medium text-white bg-blue-600 rounded-full shadow-md hover:bg-blue-700 transition whitespace-nowrap ${className}`}
    >
      {fullName}
    </span>
  );
};

const CharacterList = ({
  characters,
  listClassName,
  characterClassName,
  onClick,
}: CharacterListProps) => {
  if (!characters || characters.length === 0) return null;

  return (
    <div className={`flex flex-wrap gap-2 pb-2 ${listClassName}`}>
      {characters.map((character) => (
        <ClickableCharacter
          key={character.id}
          character={character}
          className={characterClassName}
          onClick={(e: React.MouseEvent<HTMLDivElement>) => {
            e.stopPropagation();
            if (onClick) {
              onClick(character);
            }
          }}
        />
      ))}
    </div>
  );
};

export default CharacterList;
