import NavButton from '@/components/Buttons/NavButton';
import { paths } from '@/config/paths';
import React from 'react';
import { Outlet } from 'react-router';

const Layout = () => {
  return (
    <div className="h-screen flex flex-col items-center justify-start p-6 bg-gray-100">
      <header className="relative w-full bg-white shadow-md rounded-xl py-4 mb-6 flex items-center justify-center">
        {/* Left Side Buttons */}
        <div className="absolute left-4 flex space-x-2">
          <NavButton to={paths.entries.path}>
            Entries
          </NavButton>
          <NavButton to={paths.characters.path}>
            Characters
          </NavButton>
        </div>

        {/* Title */}
        <h1 className="text-4xl font-extrabold text-gray-800">Alexandria</h1>

        {/* Right Side Buttons */}
        <div className="absolute right-4 flex space-x-2">

        </div>
      </header>

      {/* Main Content */}
      <main className="w-full max-w-6xl flex-grow bg-white shadow-lg rounded-xl p-8 overflow-hidden">
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;
