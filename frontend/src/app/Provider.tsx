import React from 'react';
import { AuthProvider } from 'react-oidc-context';
import { userManager, onSigninCallback } from '@/config/auth';
import { ProtectedRoute } from '@/app/ProtectedRoute';
import { EntriesRefreshProvider } from '@/features/entries/hooks/EntriesContext';
import { ToastProvider } from '@/hooks/ToastContext';
import { CharactersContextProvider } from '@/features/characters/hooks/CharactersContext';

type AppProviderProps = {
  children: React.ReactNode;
}

export const AppProvider = ({ children }: AppProviderProps) => {
  return (
    <AuthProvider userManager={userManager} onSigninCallback={onSigninCallback}>
      <ProtectedRoute>
        <ToastProvider>
          <CharactersContextProvider>
            <EntriesRefreshProvider>
              {children}
            </EntriesRefreshProvider>
          </CharactersContextProvider>
        </ToastProvider>
      </ProtectedRoute>
    </AuthProvider>
  );
};