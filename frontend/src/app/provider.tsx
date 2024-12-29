import { AuthProvider } from 'react-oidc-context';
import { userManager, onSigninCallback } from '@/config/auth';
import { ProtectedRoute } from '@/app/protectedRoute';

type AppProviderProps = {
  children: React.ReactNode;
}

export const AppProvider = ({ children }: AppProviderProps) => {
  return (
    <AuthProvider userManager={userManager} onSigninCallback={onSigninCallback}>
      <ProtectedRoute>
        {children}
      </ProtectedRoute>
    </AuthProvider>
  );
};