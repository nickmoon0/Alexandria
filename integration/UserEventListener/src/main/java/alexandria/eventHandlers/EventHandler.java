package alexandria.eventHandlers;

import alexandria.services.JsonService;
import alexandria.services.RabbitMqService;
import org.keycloak.events.admin.AdminEvent;

public abstract class EventHandler implements AutoCloseable {
    protected final RabbitMqService rabbitMqService;
    protected final JsonService jsonService;

    public EventHandler() {
        this.rabbitMqService = RabbitMqService.create();
        this.jsonService = new JsonService();
    }

    public abstract void execute(AdminEvent event);

    @Override
    public void close() {
        this.rabbitMqService.close();
    }
}
