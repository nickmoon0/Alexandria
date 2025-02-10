namespace Alexandria.Application.Common.Constants;

public enum FilePermissions
{
    None = 0,
    Read = 1 << 0,
    Write = 1 << 1,
}