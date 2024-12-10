using Alexandria.Application.Common.Constants;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface IFileService
{
    public ErrorOr<string> GenerateFilePath(string fileName, FileType fileType);
    public ErrorOr<FileType> DetermineFileType(string fileName);
}