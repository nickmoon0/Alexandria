using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Documents.Queries;

[Flags]
public enum GetDocumentFileStreamOptions
{
    None = 0,
    HeadersOnly = 1 << 0
}

public record GetDocumentFileStreamQuery(
    Guid DocumentId,
    GetDocumentFileStreamOptions Options = GetDocumentFileStreamOptions.None) : IRequest<ErrorOr<GetDocumentFileStreamResponse>>;

public record GetDocumentFileStreamResponse(
    FileStream? DocumentFileStream = null,
    string? FileName = null,
    string? ContentType = null);

public class GetDocumentFileStreamHandler : IRequestHandler<GetDocumentFileStreamQuery, ErrorOr<GetDocumentFileStreamResponse>>
{
    private readonly IAppDbContext _context;
    private readonly IFileService _fileService;
    private readonly ILogger<GetDocumentFileStreamHandler> _logger;
    
    public GetDocumentFileStreamHandler(
        IAppDbContext context,
        IFileService fileService,
        ILogger<GetDocumentFileStreamHandler> logger)
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
        var filePath = Path.Combine(_fileService.GetAbsoluteFileDirectory(), document.ImagePath!, fileName);
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var contentType = _fileService.GetContentType(fileName);

        if (request.Options.HasFlag(GetDocumentFileStreamOptions.HeadersOnly))
        {
            return new GetDocumentFileStreamResponse(ContentType: contentType);
        }
        
        return new GetDocumentFileStreamResponse(
            stream,
            fileName,
            contentType);
    }
}