using MediatR;

namespace OzoneMobileService.Application.Common.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
