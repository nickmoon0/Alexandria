package alexandria.providers;

import alexandria.eventHandlers.UserCreatedHandler;
import alexandria.services.EventHandlerService;
import org.keycloak.events.Event;
import org.keycloak.events.EventListenerProvider;
import org.keycloak.events.admin.AdminEvent;
import org.keycloak.events.admin.ResourceType;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class UserEventProvider implements EventListenerProvider {
    private static final Logger logger = LoggerFactory.getLogger(UserEventProvider.class);

    public UserEventProvider() {
    }

    @Override
    public void onEvent(Event event) {

    }

    @Override
    public void onEvent(AdminEvent adminEvent, boolean b) {
        var resourceTypeUser = adminEvent.getResourceType().equals(ResourceType.USER);

        // Only handle user events
        if (!resourceTypeUser) return;

        var eventType = adminEvent.getOperationType();
        logger.info("Received user event: {}", eventType);

        switch (eventType) {
            case CREATE -> EventHandlerService.ExecuteEventHandler(UserCreatedHandler.class, adminEvent);
            default -> logger.warn("No handler implemented for user event: {}", eventType);
        }

    }

    @Override
    public void close() {

    }
}
