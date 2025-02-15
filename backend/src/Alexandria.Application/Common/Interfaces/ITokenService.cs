using Alexandria.Application.Common.Constants;

namespace Alexandria.Application.Common.Interfaces;

public interface ITokenService
{
    public string GenerateToken(Guid documentId, int expiryMinutes, params FilePermissions[] filePermissions);
    public bool ValidateToken(string token, out Guid? documentId, out FilePermissions[]? filePermissions);
}