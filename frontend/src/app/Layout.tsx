import NavButton from '@/components/Buttons/NavButton';
import { paths } from '@/config/paths';
import React from 'react';
import { useAuth } from 'react-oidc-context';
import { Outlet } from 'react-router';

const Layout = () => {
  const auth = useAuth();

  const username = auth.user?.profile.preferred_username;
  const fullname = auth.user?.profile.name;

  return (
    <div className="h-screen flex flex-col items-center justify-start p-6 bg-gray-100 overflow-y-auto">
      <header className="relative w-full bg-white shadow-md rounded-xl py-4 mb-6 flex items-center justify-center">
        {/* Left Side Buttons */}
        <div className="absolute left-4 top-1/2 transform -translate-y-1/2 flex space-x-2">
          <NavButton to={paths.entries.path}>Entries</NavButton>
          <NavButton to={paths.characters.path}>Characters</NavButton>
        </div>

        {/* Title */}
        <h1 className="text-4xl font-extrabold text-gray-800">Alexandria</h1>

        {/* Right Side Buttons */}
        <div className="absolute right-4 top-0 bottom-0 flex items-center space-x-2">
          <div className="px-4 py-2 rounded-lg text-sm transition">
            <p><span className='font-medium'>{fullname}</span> ({username})</p>
          </div>
          <NavButton to={paths.logout.path}>Logout</NavButton>
        </div>
      </header>

      {/* Main Content */}
      <main className="w-full max-w-[65vw] flex-grow bg-white shadow-lg rounded-xl p-8">
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;
