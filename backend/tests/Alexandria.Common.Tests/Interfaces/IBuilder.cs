using ErrorOr;

namespace Alexandria.Common.Tests.Interfaces;

public interface IBuilder<T>
{
    public ErrorOr<T> Build();
}