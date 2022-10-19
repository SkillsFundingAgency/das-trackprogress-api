using MediatR;
using NServiceBus;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.DTOs;
using SFA.DAS.TrackProgress.Infrastructure;
using SFA.DAS.TrackProgress.Messages.Events;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application.Commands.RecordApprenticeshipProgress;

public record RecordApprenticeshipProgressCommand(
    long ProviderId,
    long Uln,
    DateTime StartDate,
    long CommitmentsApprenticeshipId,
    long? CommitmentsContinuationId,
    string StandardUid,
    ProgressItem[] Ksbs
    ) : IRequiresTransaction, IRequest<RecordApprenticeshipProgressResponse>;

public class RecordApprenticeshipProgressCommandHandler : IRequestHandler<RecordApprenticeshipProgressCommand, RecordApprenticeshipProgressResponse>
{
    private readonly TrackProgressContext _context;
    private readonly IMessageSession _messageSession;

    public RecordApprenticeshipProgressCommandHandler(TrackProgressContext context, IMessageSession messageSession)
    {
        _context = context;
        _messageSession = messageSession;
    }

    public async Task<RecordApprenticeshipProgressResponse> Handle(
        RecordApprenticeshipProgressCommand request, CancellationToken cancellationToken)
    {
        var progress = new Progress(
            new ProviderApprenticeshipIdentifier(
                request.ProviderId,
                request.Uln,
                request.StartDate),
            new ApprovalId(
                request.CommitmentsApprenticeshipId,
                request.CommitmentsContinuationId),
            request.StandardUid,
            new KsbTaxonomy(
                ToDomainTaxonomy(request.Ksbs)));

        _context.Progress.Add(progress);

        await _context.SaveChangesAsync(cancellationToken);
        await _messageSession.Publish(new NewProgressAddedEvent
            {CommitmentsApprenticeshipId = request.CommitmentsApprenticeshipId});

        return new RecordApprenticeshipProgressResponse(progress.Id);
    }

    private static KsbTaxonomyItem[] ToDomainTaxonomy(ProgressItem[] dto)
        => dto.Select(x => new KsbTaxonomyItem(x.Id, x.Value)).ToArray();
}

public record RecordApprenticeshipProgressResponse(
    long ProgressId
);