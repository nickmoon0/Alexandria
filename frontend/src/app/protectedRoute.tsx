import { initializeToken } from '@/lib/api-client';
import { ReactNode } from 'react';
import { useEffect, useState } from 'react';
import { hasAuthParams, useAuth } from 'react-oidc-context';

interface ProtectedRouteProps {
  children: ReactNode;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = (props) => {
  const { children } = props;
  const auth = useAuth();
  const [hasTriedSignin, setHasTriedSignin] = useState(false);

  useEffect(() => {
    if (!(hasAuthParams() || auth.isAuthenticated || auth.activeNavigator || auth.isLoading || hasTriedSignin)) {
      void auth.signinRedirect();
      setHasTriedSignin(true);
    }
  }, [auth, hasTriedSignin]);

  // Initialize the token when the user is authenticated
  useEffect(() => {
    if (auth.isAuthenticated && auth.user) {
      initializeToken(auth.user.access_token);
    }
  }, [auth.isAuthenticated, auth.user]);

  return (
    <>
      {auth.error ? (
        <>
          <h1>We've hit a snag</h1>
        </>
      ) : auth.isLoading ? (
        <>
          <h1>Loading...</h1>
        </>
      ) : auth.isAuthenticated ? (
        children
      ) : (
        <>
          <h1>We've hit a snag</h1>
        </>
      )}
    </>
  );
};