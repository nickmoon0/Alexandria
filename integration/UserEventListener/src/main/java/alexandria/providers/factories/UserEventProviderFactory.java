package alexandria.providers.factories;

import alexandria.providers.UserEventProvider;
import org.keycloak.Config;
import org.keycloak.events.EventListenerProvider;
import org.keycloak.events.EventListenerProviderFactory;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.KeycloakSessionFactory;

public class UserEventProviderFactory implements EventListenerProviderFactory {
    private static final String PROVIDER_ID = "custom-user-event-handler";

    @Override
    public EventListenerProvider create(KeycloakSession keycloakSession) {
        return new UserEventProvider();
    }

    @Override
    public void init(Config.Scope scope) {

    }

    @Override
    public void postInit(KeycloakSessionFactory keycloakSessionFactory) {

    }

    @Override
    public void close() {

    }

    @Override
    public String getId() {
        return PROVIDER_ID;
    }
}
