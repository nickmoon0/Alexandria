import React from 'react';
import { AppProvider } from '@/app/provider';
import { AppRouter } from '@/app/router';

export const App = () => {
  return (
    <AppProvider>
      <div className="h-screen flex flex-col items-center justify-start p-6 bg-gray-100">
        <header className="w-full bg-white shadow-md rounded-xl py-4 mb-6">
          <h1 className="text-center text-4xl font-extrabold text-gray-800">Alexandria</h1>
        </header>
        <main className="w-full max-w-6xl flex-grow bg-white shadow-lg rounded-xl p-8 overflow-hidden">
          <AppRouter />
        </main>
      </div>
    </AppProvider>
  );
};
