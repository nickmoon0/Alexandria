package alexandria.services;

import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;
import com.rabbitmq.client.Channel;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.util.concurrent.TimeoutException;

public class RabbitMqService implements AutoCloseable {
    private static final String rabbitUsername = System.getenv("RABBITMQ_USERNAME");
    private static final String rabbitPassword = System.getenv("RABBITMQ_PASSWORD");
    private static final String rabbitHost = System.getenv("RABBITMQ_HOST");
    private static final String rabbitQueue = System.getenv("RABBITMQ_QUEUE");

    private static final Logger logger = LoggerFactory.getLogger(RabbitMqService.class);

    private final Connection connection;
    private final Channel channel;

    public RabbitMqService(Connection connection, Channel channel) {
        this.connection = connection;
        this.channel = channel;
    }

    public boolean publish(String message) {
        try {
            channel.basicPublish("", rabbitQueue, null, message.getBytes());
            return true;
        } catch (IOException e) {
            return false;
        }
    }

    public static RabbitMqService create() {
        logger.info("Attempting to create RabbitMqClient...");

        var connFactory = new ConnectionFactory();

        connFactory.setUsername(rabbitUsername);
        connFactory.setPassword(rabbitPassword);
        connFactory.setHost(rabbitHost); // Hostname in docker-compose file

        try {
            var connection = connFactory.newConnection();
            var channel = connection.createChannel();
            channel.queueDeclare(rabbitQueue, true, false, false, null);

            return new RabbitMqService(connection, channel);
        } catch (IOException e) {
            logger.error("Failed to create RabbitMqClient", e);
            logger.error("IOException: {}", e.getMessage());
            return null;
        } catch (TimeoutException e) {
            logger.error("Failed to create RabbitMqClient", e);
            logger.error("TimeoutException: {}", e.getMessage());
            return null;
        }
    }

    @Override
    public void close() {
        try {
            this.channel.close();
            this.connection.close();
        } catch (IOException | TimeoutException e) {
            logger.error("Failed to close RabbitMqClient", e);
        }
    }
}
