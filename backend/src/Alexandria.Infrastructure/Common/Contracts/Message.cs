namespace Alexandria.Infrastructure.Common.Contracts;

public class Message<T> : BaseMessage
{
    public required T Data { get; set; }
}