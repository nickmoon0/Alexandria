using System.Text.Json;
using Alexandria.Application.Users.Commands.CreateUser;
using Alexandria.Application.Users.Commands.DeleteUser;
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
        var messageObj = JsonSerializer.Deserialize<Message<UserDto>>(message, options);
        if (messageObj == null)
        {
            logger.LogError("Received unknown message");
            logger.LogError("Message: {Message}", message);
            throw new Exception("Failed to deserialize message");
        }
        
        var user = messageObj.Data;
        
        switch (messageObj.Type)
        {
            case MessageConstants.OperationType.Create:
                var createUserCommand = new CreateUserCommand(user.Id, user.FirstName!, user.LastName!);
                await mediator.Send(createUserCommand);
                break;
            case MessageConstants.OperationType.Delete:
                var deleteUserCommand = new DeleteUserCommand(user.Id);
                await mediator.Send(deleteUserCommand);
                break;
            default:
                logger.LogError("Received unsupported message type: {Type}", messageObj.Type);
                break;
        }
    }
}