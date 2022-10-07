using MediatR;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Infrastructure
{
    public class TransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> 
    {
        private readonly TrackProgressContext _context;

        public TransactionPipelineBehavior(TrackProgressContext context)
        {
            _context = context;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!ShouldHandleRequest(request))
            {
                return await next();
            }

            var transaction = await _context.Database.BeginTransactionAsync(CancellationToken.None);

            try
            {
                var response = await next();
                await transaction.CommitAsync(CancellationToken.None);
                return response;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw new Exception("UoW", e);
            }
        }

        private bool ShouldHandleRequest(TRequest request) => request is IRequiresTransaction;
    }


    public interface IRequiresTransaction
    {
    }
}