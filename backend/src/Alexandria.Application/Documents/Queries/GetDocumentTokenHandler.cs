using Alexandria.Application.Common.Constants;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Documents.Queries;

public record GetDocumentTokenRequest(
    Guid DocumentId,
    FilePermissions[] Permissions,
    int ExpiryMinutes = 5) : IRequest<ErrorOr<GetDocumentTokenResponse>>;
public record GetDocumentTokenResponse(string Token);

public class GetDocumentTokenHandler : IRequestHandler<GetDocumentTokenRequest, ErrorOr<GetDocumentTokenResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<GetDocumentTokenHandler> _logger;

    public GetDocumentTokenHandler(IAppDbContext context, ITokenService tokenService, ILogger<GetDocumentTokenHandler> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<ErrorOr<GetDocumentTokenResponse>> Handle(GetDocumentTokenRequest request, CancellationToken cancellationToken)
    {
        if (request.ExpiryMinutes is <= 0 or > 15)
        {
            _logger.LogInformation("Token expiry time is invalid");
            return Error.Validation();
        }
        
        var documentExists = await _context.Documents
            .AnyAsync(document => document.Id == request.DocumentId, cancellationToken: cancellationToken);
        if (!documentExists)
        {
            _logger.LogInformation("Document with ID {ID} not found", request.DocumentId);
            return DocumentErrors.NotFound;
        }
        
        var token = _tokenService.GenerateToken(request.DocumentId, request.ExpiryMinutes, request.Permissions);
        return new GetDocumentTokenResponse(token);
    }
}