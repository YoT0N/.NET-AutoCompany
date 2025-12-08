using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;

namespace PersonnelService.Application.Common.Behaviors
{
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation failed for {RequestName}. Errors: {@Errors}",
                    typeof(TRequest).Name, ex.Errors);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Not found exception for {RequestName}. Message: {Message}",
                    typeof(TRequest).Name, ex.Message);
                throw;
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning("Conflict exception for {RequestName}. Message: {Message}",
                    typeof(TRequest).Name, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {RequestName}. Request data: {@Request}",
                    typeof(TRequest).Name, request);
                throw;
            }
        }
    }
}