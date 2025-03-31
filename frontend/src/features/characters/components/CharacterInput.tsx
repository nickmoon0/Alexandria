import React, { FC, useState, ChangeEvent, KeyboardEvent } from 'react';
//import { getCharacter } from '@/features/characters/api/get-character';
import { getCharacters } from '@/features/characters/api/get-characters';
import { Character } from '@/types/app';
import { useToast } from '@/hooks/ToastContext';
import { ToastType } from '@/components/Toast';
import { PaginatedRequest } from '@/types/pagination';
//import { createCharacter } from '@/features/characters/api/create-character';

export interface CharacterInputProps {
  initialCharacters: Character[];
  onCharacter: (character: Character) => Promise<boolean>;
  onCharacterRemove: (character: Character) => Promise<boolean>;
}

const CharacterInput: FC<CharacterInputProps> = ({ initialCharacters, onCharacter, onCharacterRemove }) => {
  const [characters, setCharacters] = useState<Character[]>(initialCharacters);
  const [input, setInput] = useState<string>('');
  const [suggestions, setSuggestions] = useState<Character[]>([]);

  const { showToast } = useToast();

  // Fetch character suggestions from an API based on the query.
  const searchCharacters = async (query: string): Promise<void> => {
    const pageRequest: PaginatedRequest = {
      PageSize: 3,
    };
    const results = await getCharacters({ pageRequest, searchString: query });
    if (query.trim().length > 0) {
      setSuggestions(results.data);
    } else {
      setSuggestions([]);
    }
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>): void => {
    if (e.key === 'Enter' && input.trim() !== '') {
      const newCharacterName = input.trim();

      // Check if the first suggestion matches the entered full name.
      const characterToAdd =
        suggestions.length > 0 &&
        // For simplicity, we concatenate firstName and lastName.
        (`${suggestions[0].firstName} ${suggestions[0].lastName}`.trim() === newCharacterName)
          ? suggestions[0]
          : null;

      if (characterToAdd === null) {
        showToast('Not found', ToastType.Error);
        //handleCreateCharacter(newCharacterName);
      } else if (!characters.map(character => character.id).includes(characterToAdd.id)) {
        onCharacter(characterToAdd).then(success => {
          if (success) {
            setCharacters([...characters, characterToAdd]);
          }
        });
      }
      setInput('');
      setSuggestions([]);
    } else if (e.key === 'Backspace' && input === '' && characters.length > 0) {
      onCharacterRemove(characters[characters.length - 1]).then(success => {
        if (success) {
          setCharacters(characters.slice(0, characters.length - 1));
        }
      });
    }
  };

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>): void => {
    const newValue = e.target.value;
    setInput(newValue);
    searchCharacters(newValue);
  };

  const removeCharacter = (characterToRemove: Character): void => {
    onCharacterRemove(characterToRemove).then(success => {
      if (success) {
        setCharacters(characters.filter(character => character.id !== characterToRemove.id));
      }
    });
  };

  const handleSuggestionClick = (suggestion: Character): void => {
    if (!characters.some(character => character.id === suggestion.id)) {
      onCharacter(suggestion).then(success => {
        if (success) {
          setCharacters([...characters, suggestion]);
        }
      });
    }
    setInput('');
    setSuggestions([]);
  };

  return (
    <div>
      <div className="flex flex-wrap gap-2">
        {characters.map((character, index) => (
          <div
            key={index}
            className="flex items-center bg-blue-600 text-white rounded-full px-3 py-1"
          >
            <span>{character.firstName} {character.lastName}</span>
            <button
              type="button"
              onClick={() => removeCharacter(character)}
              className="ml-2 text-white focus:outline-none"
            >
              &times;
            </button>
          </div>
        ))}
        <div className="relative">
          <input
            type="text"
            className="flex-grow p-1 outline-none border border-gray-300 rounded"
            placeholder="Add character"
            value={input}
            onChange={handleInputChange}
            onKeyDown={handleKeyDown}
          />
          {suggestions.length > 0 && (
            <ul className="absolute bg-white border border-gray-300 w-full mt-1 rounded-md shadow-lg z-10">
              {suggestions.map((suggestion, index) => (
                <li
                  key={index}
                  onClick={() => handleSuggestionClick(suggestion)}
                  className="cursor-pointer hover:bg-gray-100 px-4 py-2"
                >
                  {suggestion.firstName} {suggestion.lastName}
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
};

export default CharacterInput;
