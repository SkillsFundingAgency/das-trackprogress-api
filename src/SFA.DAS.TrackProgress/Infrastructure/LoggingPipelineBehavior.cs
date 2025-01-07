using MediatR;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.TrackProgress.Infrastructure;
public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            _logger.LogInformation("Start handling '{Type}'", typeof(TRequest));
            var response = await next();
            _logger.LogInformation("End handling '{Type}'", typeof(TRequest));
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling '{Type}' with request: {@Request}", typeof(TRequest), request);
            throw new InvalidOperationException($"Error handling '{typeof(TRequest)}'", e);
        }
    }
}
