using MediatR;

namespace PersonnelService.Application.Common.Interfaces
{
    // Command з результатом
    public interface ICommand<TResponse> : IRequest<TResponse> { }
}