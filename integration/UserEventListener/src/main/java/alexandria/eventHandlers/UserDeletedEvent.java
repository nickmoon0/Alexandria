package alexandria.eventHandlers;

import alexandria.dtos.UserDto;
import alexandria.eventHandlers.common.EventHandler;
import alexandria.eventHandlers.common.EventHelpers;
import org.keycloak.events.admin.AdminEvent;
import org.keycloak.events.admin.OperationType;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class UserDeletedEvent extends EventHandler {

    private static final Logger logger = LoggerFactory.getLogger(UserDeletedEvent.class);

    public UserDeletedEvent() { super(); }

    @Override
    public void execute(AdminEvent event) {
        var id = EventHelpers.getUserId(event.getResourcePath());

        logger.info("User deleted event: {}", id);
        var userObj = new UserDto();
        userObj.id = id;
        var messageObj = new Message<>(OperationType.DELETE.name(), userObj);

        var payload = messageService.createJson(messageObj);

        if(payload != null && rabbitMqService.publish(payload)) {
            logger.info("Published user deletion message");
        } else {
            logger.error("Failed to publish user deletion message");
        }
    }
}
