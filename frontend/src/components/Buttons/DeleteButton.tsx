import React from 'react';
import { CircleX } from 'lucide-react';

export interface DeleteButtonProps {
  disabled?: boolean;
  onClick: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

const DeleteButton = ({ disabled = false, onClick }: DeleteButtonProps) => {
  return (
    <button 
      disabled={disabled}
      onClick={onClick}
      className={`px-2 py-2 text-red-600 font-medium rounded-lg shadow-md transition 
        ${disabled ? 'opacity-50 cursor-not-allowed' : 'hover:bg-red-700 hover:text-white'}`}
    >
      <CircleX />
    </button>
  );
};

export default DeleteButton;
