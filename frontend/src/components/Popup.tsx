import React from 'react';
import Button from '@/components/Buttons/Button';
import { SquareX } from 'lucide-react';

interface PopupProps {
  title?:string,
  children:React.ReactNode;
  controlChildren?:React.ReactNode;
  controlChildrenOnLeft?:boolean
  onClose: () => void; // Function to close popup
};

const Popup = ({
  title, 
  children, 
  controlChildren,
  controlChildrenOnLeft = true,
  onClose
}:PopupProps) => {
  return (
    <div className='fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50'>
      <div className='bg-white p-6 rounded-2xl shadow-2xl max-w-lg w-full relative animate-fadeIn'>
        
        {/* Close Button placed naturally instead of absolute positioning */}
        <div className={`flex ${title ? 'justify-between' : 'justify-end'}`}>
          { title && <h1 className='text-2xl font-bold text-gray-800'>{title}</h1> }
          <button
            className='text-gray-500 hover:text-gray-800'
            onClick={onClose}
          >
            <SquareX />
          </button>
        </div>
        { children }
        
        {/* Footer controls section */}
        <div className='flex justify-between items-center pt-4'>
          {controlChildrenOnLeft && 
            controlChildren
          }
          <Button onClick={onClose} className='h-auto self-end'>
            Close
          </Button>
          {!controlChildrenOnLeft &&
            controlChildren}
        </div>
      </div>
    </div>
  );
};

export default Popup;