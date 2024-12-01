namespace Alexandria.Infrastructure.Common.Contracts;

public class Message<T>
{
    public required string Type { get; set; }
    public required T Data { get; set; }
}