package alexandria.eventHandlers.common;

import alexandria.services.MessageService;
import alexandria.services.RabbitMqService;
import org.keycloak.events.admin.AdminEvent;

public abstract class EventHandler implements AutoCloseable {
    protected final RabbitMqService rabbitMqService;
    protected final MessageService messageService;

    public EventHandler() {
        this.rabbitMqService = RabbitMqService.create();
        this.messageService = new MessageService();
    }

    public abstract void execute(AdminEvent event);

    @Override
    public void close() {
        this.rabbitMqService.close();
    }

    public static class Message<T> {
        public String type;
        public T data;

        public Message(String type, T data) {
            this.type = type;
            this.data = data;
        }
    }
}
