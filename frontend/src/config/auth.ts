import { UserManager, WebStorageStateStore } from 'oidc-client-ts';
import { env } from '@/config/env';

export const userManager = new UserManager({
  authority: env.KEYCLOAK_URL,
  client_id: env.KEYCLOAK_CLIENT_ID,
  redirect_uri: `${window.location.origin}${window.location.pathname}`,
  post_logout_redirect_uri: window.location.origin,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  monitorSession: true
});

export const onSigninCallback = () => {
  window.history.replaceState({}, document.title, window.location.pathname);
};