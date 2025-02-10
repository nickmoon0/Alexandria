using Alexandria.Application.Common.Constants;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Infrastructure.Common.Options;
using ErrorOr;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Alexandria.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly FileStorageOptions _options;

    public FileService(
        IDateTimeProvider dateTimeProvider,
        IOptions<FileStorageOptions> options)
    {
        _dateTimeProvider = dateTimeProvider;
        _options = options.Value;
    }
    
    public ErrorOr<string> GenerateFilePath(string fileName, FileType fileType)
    {
        var currYear = _dateTimeProvider.UtcNow.Year;
        var directory = Path.Combine(_options.Path, fileType.ToString(), currYear.ToString());
        var filePath = Path.Combine(directory, fileName);
        
        // Make sure that directory exists
        Directory.CreateDirectory(directory);

        return filePath;
    }

    public ErrorOr<FileType> DetermineFileType(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        
        if (FileExtensions.Images.Extensions.Contains(extension)) return FileType.Image;
        if (FileExtensions.Videos.Extensions.Contains(extension)) return FileType.Video;
        if (FileExtensions.Documents.Extensions.Contains(extension)) return FileType.Document;

        return Error.NotFound();
    }

    public string GetContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
}