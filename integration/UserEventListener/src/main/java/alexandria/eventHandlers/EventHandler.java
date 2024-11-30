package alexandria.eventHandlers;

import alexandria.services.JsonService;
import alexandria.services.RabbitMqService;

public abstract class EventHandler {
    protected final RabbitMqService rabbitMqService;
    protected final JsonService jsonService;

    public EventHandler() {
        this.rabbitMqService = RabbitMqService.create();
        this.jsonService = new JsonService();
    }
}
