import React from 'react';
import { AuthProvider } from 'react-oidc-context';
import { userManager, onSigninCallback } from '@/config/auth';
import { ProtectedRoute } from '@/app/protectedRoute';
import { EntriesRefreshProvider } from '@/features/entries/hooks/EntriesContext';
import { ToastProvider } from '@/hooks/ToastContext';

type AppProviderProps = {
  children: React.ReactNode;
}

export const AppProvider = ({ children }: AppProviderProps) => {
  return (
    <AuthProvider userManager={userManager} onSigninCallback={onSigninCallback}>
      <ProtectedRoute>
        <ToastProvider>
          <EntriesRefreshProvider>
            {children}
          </EntriesRefreshProvider>
        </ToastProvider>
      </ProtectedRoute>
    </AuthProvider>
  );
};