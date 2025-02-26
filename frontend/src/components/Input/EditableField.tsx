import React, { useState, useEffect } from 'react';

export interface EditableFieldProps {
  value: string;
  onChange?: (value?: string) => void;
  placeholder?: string;
  className?: string;
  textClassName?: string;
}

const EditableField: React.FC<EditableFieldProps> = ({
  value,
  onChange,
  placeholder = 'Click to edit...',
  className,
  textClassName
}) => {
  const [isEditing, setIsEditing] = useState<boolean>(false);
  const [inputValue, setInputValue] = useState<string>(value ?? '');

  // Sync state with prop changes
  useEffect(() => {
    setInputValue(value ?? '');
  }, [value]);

  const handleBlur = () => {
    setIsEditing(false);
    if (onChange) {
      onChange(inputValue);
    }
  };

  const onKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key.toLowerCase() === 'enter') {
      e.preventDefault();
      handleBlur();
    }
  };

  return (
    <div
      className={`inline-block rounded-lg transition cursor-pointer ${
        isEditing
          ? 'px-2 py-2 focus-within:ring-2 focus-within:ring-blue-500 focus-within:border-blue-500'
          : 'hover:bg-gray-100'
      } ${className}`}
      onClick={() => setIsEditing(true)}
    >
      {isEditing ? (
        <input
          type="text"
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
          onBlur={handleBlur}
          onKeyDown={onKeyDown}
          autoFocus
          className={`w-auto focus:outline-none ${textClassName}`}
          style={{ width: `${(inputValue?.length ?? 0) + 1}ch` }}
        />
      ) : (
        <span className={textClassName}>
          {inputValue || placeholder}
        </span>
      )}
    </div>
  );
};

export default EditableField;
