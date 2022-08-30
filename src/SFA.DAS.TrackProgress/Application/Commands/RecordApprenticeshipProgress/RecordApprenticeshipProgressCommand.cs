using MediatR;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.DTOs;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application.Commands.RecordApprenticeshipProgress;

public record RecordApprenticeshipProgressCommand(
    long ProviderId,
    long Uln,
    DateTime StartDate,
    long CommitmentsApprenticeshipId,
    long? CommitmentsContinuationId,
    ProgressItem[] Ksbs) : IRequest;

public class RecordApprenticeshipProgressCommandHandler : IRequestHandler<RecordApprenticeshipProgressCommand>
{
    private readonly TrackProgressContext context;

    public RecordApprenticeshipProgressCommandHandler(TrackProgressContext context)
        => this.context = context;

    public async Task<Unit> Handle(
        RecordApprenticeshipProgressCommand request, CancellationToken cancellationToken)
    {
        context.Progress.Add(
            new Progress(
                new ProviderApprenticeshipIdentifier(
                    request.ProviderId,
                    request.Uln,
                    request.StartDate),
                new ApprovalId(
                    request.CommitmentsApprenticeshipId,
                    request.CommitmentsContinuationId),
                new KsbTaxonomy(
                    ToDomainTaxonomy(request.Ksbs))));

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static KsbTaxonomyItem[] ToDomainTaxonomy(ProgressItem[] dto)
        => dto.Select(x => new KsbTaxonomyItem(x.Id, x.Value)).ToArray();
}