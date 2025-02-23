import React from 'react';
import { AppProvider } from '@/app/provider';
import { AppRouter } from '@/app/router';

export const App = () => {
  return (
    <AppProvider>
      <div className="min-h-screen bg-gray-100 flex flex-col items-center justify-start p-6">
        <header className="w-full bg-white shadow-md rounded-xl py-4 mb-6">
          <h1 className="text-center text-4xl font-extrabold text-gray-800">Alexandria</h1>
        </header>
        <main className="w-full max-w-6xl bg-white shadow-lg rounded-xl p-8">
          <AppRouter />
        </main>
      </div>
    </AppProvider>
  );
};