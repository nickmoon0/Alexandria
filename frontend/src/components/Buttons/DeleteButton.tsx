import React from 'react';
import { CircleX } from 'lucide-react';

export interface DeleteButtonProps {
  onClick: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

const DeleteButton = ({ onClick }: DeleteButtonProps) => {
  return (
    <button onClick={onClick} className='px-2 py-2 text-red-600 font-medium rounded-lg shadow-md hover:bg-red-700 hover:text-white transition'>
      <CircleX />
    </button>
  );
};

export default DeleteButton;