using MediatR;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.TrackProgress.Infrastructure;
public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public LoggingPipelineBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            _logger.LogInformation("Start handling '{type}'", typeof(TRequest));
            var response = await next();
            _logger.LogInformation("End handling '{type}'", typeof(TRequest));
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling '{type}'", typeof(TRequest));
            throw;
        }
    }
}
