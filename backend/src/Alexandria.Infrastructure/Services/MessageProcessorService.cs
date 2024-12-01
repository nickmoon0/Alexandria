using System.Text.Json;
using Alexandria.Application.Users.Commands.CreateUser;
using Alexandria.Infrastructure.Common;
using Alexandria.Infrastructure.Common.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Infrastructure.Services;

public class MessageProcessorService(
    ILogger<MessageProcessorService> logger,
    IMediator mediator)
{
    public async Task HandleMessage(string message)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var baseMessage = JsonSerializer.Deserialize<BaseMessage>(message, options);
        if (baseMessage == null)
        {
            logger.LogError("Received unknown message");
            logger.LogError("Message: {Message}", message);
            throw new Exception("Failed to deserialize message");
        }
        
        switch (baseMessage.Type)
        {
            case MessageConstants.OperationType.Create:
                var messageObj = JsonSerializer.Deserialize<Message<UserDto>>(message, options) 
                                 ?? throw new Exception("Failed to deserialize message");
                var command = new CreateUserCommand(messageObj.Data.FirstName!, messageObj.Data.LastName!);
                await mediator.Send(command);
                break;
            default:
                logger.LogError("Received unsupported message type: {Type}", baseMessage.Type);
                break;
        }
    }
}