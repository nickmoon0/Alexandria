import React from 'react';
import { useEffect } from 'react';
import { useAuth } from 'react-oidc-context';

const LogoutRoute = () => {
  const auth = useAuth();

  useEffect(() => {
    if (auth.isAuthenticated) {
      auth.signoutRedirect();
    }
  }, [auth]);

  return <p>Logging out...</p>;
};

export default LogoutRoute;
