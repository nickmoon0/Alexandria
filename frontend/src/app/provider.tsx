import { AuthProvider } from 'react-oidc-context';
import { userManager, onSigninCallback } from '@/config/auth';
import { ProtectedRoute } from '@/app/protectedRoute';
import { EntriesRefreshProvider } from '@/features/entries/hooks/EntriesContext';

type AppProviderProps = {
  children: React.ReactNode;
}

export const AppProvider = ({ children }: AppProviderProps) => {
  return (
    <AuthProvider userManager={userManager} onSigninCallback={onSigninCallback}>
      <ProtectedRoute>
        <EntriesRefreshProvider>
          {children}
        </EntriesRefreshProvider>
      </ProtectedRoute>
    </AuthProvider>
  );
};