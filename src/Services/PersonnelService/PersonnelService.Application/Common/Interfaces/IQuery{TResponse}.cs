using MediatR;

namespace PersonnelService.Application.Common.Interfaces
{
    public interface IQuery<TResponse> : IRequest<TResponse> { }
}