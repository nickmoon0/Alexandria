using Alexandria.Application.Common.Constants;

namespace Alexandria.FileApi.Common;

public abstract class EndpointBase
{
    protected static Guid DocumentId { get; private set; }
    protected static IReadOnlyList<FilePermissions> FilePermissions { get; private set; } = null!;

    public static void InitializeTokenParameters(Guid documentId, FilePermissions[] filePermissions)
    {
        DocumentId = documentId;
        FilePermissions = filePermissions;
    }
}