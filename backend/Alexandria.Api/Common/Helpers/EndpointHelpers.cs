using Alexandria.Api.Common.Models;

namespace Alexandria.Api.Common.Helpers;

public abstract class EndpointHelpers
{
    public static int MapErrorTypeToStatusCode(ErrorType errorType) => 
        errorType switch
        {
            ErrorType.EntityValidationFailed => StatusCodes.Status400BadRequest,
            ErrorType.RequestValidationFailed => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    
    
}