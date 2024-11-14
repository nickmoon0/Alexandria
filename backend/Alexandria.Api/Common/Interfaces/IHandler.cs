using Alexandria.Api.Common.Models;

namespace Alexandria.Api.Common.Interfaces;

public interface IHandler<in TRequest, TResponse>
{
    public Task<Result<TResponse>> Handle(TRequest request);
}