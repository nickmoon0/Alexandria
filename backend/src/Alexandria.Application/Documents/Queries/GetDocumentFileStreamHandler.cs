using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Documents.Queries;

public record GetDocumentFileStreamQuery(Guid DocumentId) : IRequest<ErrorOr<GetDocumentFileStreamResponse>>;

public record GetDocumentFileStreamResponse(FileStream DocumentFileStream, string FileName, string ContentType);

public class GetDocumentFileStreamHandler : IRequestHandler<GetDocumentFileStreamQuery, ErrorOr<GetDocumentFileStreamResponse>>
{
    private readonly IAppDbContext _context;
    private readonly IFileService _fileService;
    private readonly ILogger<GetDocumentFileStreamHandler> _logger;
    
    public GetDocumentFileStreamHandler(IAppDbContext context, IFileService fileService, ILogger<GetDocumentFileStreamHandler> logger)
    {
        _context = context;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<ErrorOr<GetDocumentFileStreamResponse>> Handle(GetDocumentFileStreamQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents.FindAsync([request.DocumentId], cancellationToken);
        if (document == null)
        {
            _logger.LogInformation("Document with ID \'{ID}\' not found", request.DocumentId);
            return DocumentErrors.NotFound;
        }
        
        // Prepare file stream
        var fileName = $"{document.Name}{document.FileExtension}";
        var filePath = Path.Combine(document.ImagePath!, fileName);
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var contentType = _fileService.GetContentType(fileName);
        
        return new GetDocumentFileStreamResponse(
            stream,
            fileName,
            contentType);
    }
}