namespace Alexandria.CoreApi.Common;

public abstract class EndpointBase
{
    protected static Guid? UserId { get; private set; }

    public static void InitializeUserId(Guid? userId)
    {
        UserId = userId;
    }
}