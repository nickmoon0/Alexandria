package alexandria.eventHandlers;

import alexandria.dtos.UserDto;
import org.keycloak.events.admin.AdminEvent;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class UserCreatedHandler extends EventHandler{

    private static final Logger logger = LoggerFactory.getLogger(UserCreatedHandler.class);

    public UserCreatedHandler() {
        super();
    }

    public void Execute(AdminEvent event) {
        var representation = event.getRepresentation();
        var id = EventHelpers.getUserId(event.getResourcePath());

        logger.info("User created event: {}", id);

        var payload = jsonService.createJson(representation, id, UserDto.class);

        if(payload != null && rabbitMqService.publish(payload)) {
            logger.info("Published user creation message");
        } else {
            logger.error("Failed to publish user creation message");
        }
    }
}
