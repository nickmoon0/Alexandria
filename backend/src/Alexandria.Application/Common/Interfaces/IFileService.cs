using Alexandria.Application.Common.Constants;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface IFileService
{ 
    public ErrorOr<string> GenerateRelativeFilePath(string fileName, FileType fileType);
    public string GetAbsoluteFileDirectory();
    public ErrorOr<FileType> DetermineFileType(string fileName);
    public string GetContentType(string fileName);
}