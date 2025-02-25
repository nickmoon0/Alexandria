import React from 'react';
import { useLocation, useNavigate } from 'react-router';

interface NavButtonProps {
  children: React.ReactNode;
  to: string;
}

const NavButton: React.FC<NavButtonProps> = ({ children, to }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const isActive = location.pathname === to;

  return (
    <button
      className={`px-4 py-2 rounded-lg text-lg font-medium transition 
        ${isActive ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-800 hover:bg-gray-300'}`}
      onClick={() => navigate(to)}
    >
      {children}
    </button>
  );
};

export default NavButton;
