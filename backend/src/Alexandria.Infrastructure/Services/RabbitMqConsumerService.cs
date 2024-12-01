using System.Text;
using System.Text.Json;
using Alexandria.Application.Users.Commands.CreateUser;
using Alexandria.Infrastructure.Common;
using Alexandria.Infrastructure.Common.Contracts;
using Alexandria.Infrastructure.Common.Options;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Alexandria.Infrastructure.Services;

public class RabbitMqConsumerService(
    ILogger<RabbitMqConsumerService> logger,
    IOptions<RabbitMqOptions> optionsInterface,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private IConnection _connection = null!;
    private IChannel _channel = null!;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Setup queue, connection and channel
        var options = optionsInterface.Value;
        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            UserName = options.UserName,
            Password = options.Password
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(
            queue: options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);
        
        // Create message consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            // Process the message here
            logger.LogInformation("Received: {message}", message);
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<MessageProcessorService>();
                await scopedProcessingService.HandleMessage(message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured");
                return;
            }
            
            // Acknowledge message
            await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
        };

        logger.LogInformation("Starting consumer...");
        
        // Start consuming
        await _channel.BasicConsumeAsync(
            queue: options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }
    
    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        logger.LogInformation("Disposed channel and connection");
    }
}