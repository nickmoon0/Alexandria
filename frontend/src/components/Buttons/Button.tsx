import React from 'react';

export interface ButtonProps<T extends (...args: unknown[]) => void> {
  onClick: T;
  children: React.ReactNode;
  className?: string;
  disabled?: boolean;
}

const Button = <T extends (...args: unknown[]) => void>({ 
  children,
  onClick,
  className,
  disabled = false 
}:ButtonProps<T>) => {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      className={`px-4 py-2 bg-blue-600 text-white font-medium rounded-lg shadow-md hover:bg-blue-700 transition ${
        disabled ? 'opacity-50 cursor-not-allowed' : ''
      } ${className}`}>
      {children}
    </button>
  );
};

export default Button;