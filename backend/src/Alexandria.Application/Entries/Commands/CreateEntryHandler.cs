using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Commands;

public record CreateEntryCommand(
    string EntryName,
    string FileName,
    Guid CreatedById,
    string? Description = null) : IRequest<ErrorOr<CreateEntryResponse>>;
public record CreateEntryResponse(string PermanentFilePath, Guid EntryId, Guid DocumentId);

public class CreateEntryHandler : IRequestHandler<CreateEntryCommand, ErrorOr<CreateEntryResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateEntryHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileService _fileService;
    
    public CreateEntryHandler(
        IAppDbContext context,
        ILogger<CreateEntryHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IFileService fileService)
    {
        _context = context;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _fileService = fileService;
    }

    public async Task<ErrorOr<CreateEntryResponse>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
    {
        // Security: Only permit specific file extensions to prevent injections
        var fileTypeResult = _fileService.DetermineFileType(request.FileName);
        if (fileTypeResult.IsError)
        {
            _logger.LogError("File type not supported: {FileExtension}", Path.GetExtension(request.FileName));
            return DocumentErrors.InvalidFileExtension;
        }
        
        // Get new file values and generate file directory
        var fileType = fileTypeResult.Value;
        var fileExtension = Path.GetExtension(request.FileName);
        var newFileName = Guid.NewGuid().ToString();
        var outputFileName = $"{newFileName}{fileExtension}";
        
        var filePathResult = _fileService.GenerateFilePath(outputFileName, fileType);
        if (filePathResult.IsError)
        {
            _logger.LogError("Failed to retrieve file path");
            return filePathResult.Errors;
        }
        
        // Create database records relevant to file
        var imagePath = Path.GetDirectoryName(filePathResult.Value);
        if (imagePath == null)
        {
            _logger.LogError("Failed to get image directory");
            return Error.Failure();
        }
        
        var entryResult = Entry.Create(
            request.EntryName,
            request.CreatedById,
            _dateTimeProvider,
            request.Description);
        if (entryResult.IsError)
        {
            _logger.LogError("Failed to create entry");
            return entryResult.Errors;
        }
        var entry = entryResult.Value;
        
        var documentResult = Document.Create(
            entry.Id,
            newFileName,
            fileExtension,
            imagePath,
            request.CreatedById,
            _dateTimeProvider);
        
        if (documentResult.IsError)
        {
            _logger.LogError("Failed to create document");
            return documentResult.Errors;
        }
        var document = documentResult.Value;
        
        await _context.Documents.AddAsync(document, cancellationToken);
        await _context.Entries.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        var response = new CreateEntryResponse(filePathResult.Value, entry.Id, document.Id);
        return response;
    }
}