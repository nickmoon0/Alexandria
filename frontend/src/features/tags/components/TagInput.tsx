import React, { FC, useState, ChangeEvent, KeyboardEvent } from 'react';
import { getTags } from '@/features/tags/api/get-tags';
import { Tag } from '@/types/app';

export interface TagInputProps {
  initialTags:Tag[];
}

const TagInput: FC<TagInputProps> = ({ initialTags }) => {
  const [tags, setTags] = useState<Tag[]>(initialTags);
  const [input, setInput] = useState<string>('');
  const [suggestions, setSuggestions] = useState<Tag[]>([]);

  // Stub function to simulate an API call for tag suggestions.
  const searchTags = async (query: string): Promise<void> => {
    console.log('Searching for tags:', query);
    
    const tags = await getTags({ searchString:query, maxCount:3 });

    if (query.trim().length > 0) {
      setSuggestions(tags);
    } else {
      setSuggestions([]);
    }
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>): void => {
    if (e.key === 'Enter' && input.trim() !== '') {
      const newTagText = input.trim();

      const tagToAdd =
        suggestions.length > 0 && suggestions[0].name === newTagText
          ? suggestions[0]
          : suggestions[0];
  
      if (!tags.map(tag => tag.name).includes(tagToAdd.name)) {
        setTags([...tags, tagToAdd]);
      }
      setInput('');
      setSuggestions([]);
    } else if (e.key === 'Backspace' && input === '' && tags.length > 0) {
      setTags(tags.slice(0, tags.length - 1));
    }
  };
  
  const handleInputChange = (e: ChangeEvent<HTMLInputElement>): void => {
    const newValue = e.target.value;
    setInput(newValue);
    searchTags(newValue);
  };

  const removeTag = (indexToRemove: number): void => {
    setTags(tags.filter((_, index) => index !== indexToRemove));
  };

  const handleSuggestionClick = (suggestion: Tag): void => {
    if (!tags.includes(suggestion)) {
      setTags([...tags, suggestion]);
    }
    setInput('');
    setSuggestions([]);
  };

  return (
    <div>
      <div className="flex flex-wrap gap-2">
        {tags.map((tag: Tag, index: number) => (
          <div
            key={index}
            className="flex items-center bg-blue-600 text-white rounded-full px-3 py-1"
          >
            <span>{tag.name}</span>
            <button
              type="button"
              onClick={() => removeTag(index)}
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
            placeholder="Add tag"
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
                  {suggestion.name}
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
};

export default TagInput;
