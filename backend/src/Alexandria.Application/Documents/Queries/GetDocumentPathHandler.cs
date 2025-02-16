using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Documents.Queries;

public record GetDocumentPathQuery(Guid DocumentId) : IRequest<ErrorOr<GetDocumentPathResponse>>;

public record GetDocumentPathResponse(Guid DocumentId, string Path);

public class GetDocumentPathHandler : IRequestHandler<GetDocumentPathQuery, ErrorOr<GetDocumentPathResponse>>
{
    private readonly IAppDbContext _context;
    private readonly IFileService _fileService;
    private readonly ILogger<GetDocumentPathHandler> _logger;

    public GetDocumentPathHandler(IAppDbContext context, IFileService fileService, ILogger<GetDocumentPathHandler> logger)
    {
        _context = context;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<ErrorOr<GetDocumentPathResponse>> Handle(GetDocumentPathQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents.FindAsync([request.DocumentId], cancellationToken);
        if (document == null)
        {
            _logger.LogInformation("Document with ID {ID} not found", request.DocumentId);
            return DocumentErrors.NotFound;
        }

        var documentPath = Path.Join(
            _fileService.GetAbsoluteFileDirectory(),
            document.ImagePath,
            $"{document.Name}{document.FileExtension}");
        
        return new GetDocumentPathResponse(document.Id, documentPath);
    }
}