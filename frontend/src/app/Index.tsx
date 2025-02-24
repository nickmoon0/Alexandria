import React from 'react';
import { AppProvider } from '@/app/Provider';
import { AppRouter } from '@/app/Router';

export const App = () => {
  return (
    <AppProvider>
      <AppRouter />
    </AppProvider>
  );
};
