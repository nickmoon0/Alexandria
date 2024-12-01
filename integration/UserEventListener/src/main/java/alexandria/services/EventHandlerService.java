package alexandria.services;

import alexandria.eventHandlers.EventHandler;
import org.keycloak.events.admin.AdminEvent;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.lang.reflect.InvocationTargetException;

public class EventHandlerService {
    private static final Logger logger = LoggerFactory.getLogger(EventHandlerService.class);

    public static <T extends EventHandler> void ExecuteEventHandler(Class<T> clazz, AdminEvent event) {
        try(T handlerInstance = clazz.getDeclaredConstructor().newInstance()) {
            handlerInstance.execute(event);
        } catch (NoSuchMethodException e) {
            logger.error("NoSuchMethodException", e);
        } catch (IllegalAccessException | InstantiationException | InvocationTargetException e) {
            logger.error("Could not access .newInstance");
        }
    }
}
