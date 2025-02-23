import React from 'react';
export interface TextAreaProps {
  value: string;
  onChange?: (value: string) => void;
  onKeyDown?: (e:React.KeyboardEvent<HTMLTextAreaElement>) => void;
  placeholder?: string;
  className?: string;
  disabled?: boolean;
  rows?: number;
}

const TextArea: React.FC<TextAreaProps> = ({
  value,
  onChange,
  onKeyDown,
  placeholder = 'Enter text...',
  className,
  disabled = false,
  rows = 4,
}) => {
  return (
    <textarea
      value={value}
      onChange={onChange ? (e) => onChange(e.target.value) : undefined}
      onKeyDown={onKeyDown ? (e) => onKeyDown(e) : undefined}
      placeholder={placeholder}
      disabled={disabled}
      rows={rows}
      className={`px-4 py-2 border border-gray-300 rounded-lg shadow-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition disabled:opacity-50 disabled:cursor-not-allowed ${className}`}
    />
  );
};

export default TextArea;
