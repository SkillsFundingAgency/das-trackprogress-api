using MediatR;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.DTOs;
using SFA.DAS.TrackProgress.Infrastructure;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application.Commands.RecordApprenticeshipProgress;

public record RecordApprenticeshipProgressCommand(
    long ProviderId,
    long Uln,
    DateTime StartDate,
    long CommitmentsApprenticeshipId,
    long? CommitmentsContinuationId,
    ProgressItem[] Ksbs) : IRequiresTransaction, IRequest<RecordApprenticeshipProgressResponse>;

public class RecordApprenticeshipProgressCommandHandler : IRequestHandler<RecordApprenticeshipProgressCommand, RecordApprenticeshipProgressResponse>
{
    private readonly TrackProgressContext context;

    public RecordApprenticeshipProgressCommandHandler(TrackProgressContext context)
        => this.context = context;

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
            new KsbTaxonomy(
                ToDomainTaxonomy(request.Ksbs)));

        context.Progress.Add(progress);

        await context.SaveChangesAsync(cancellationToken);

        return new RecordApprenticeshipProgressResponse(progress.Id);
    }

    private static KsbTaxonomyItem[] ToDomainTaxonomy(ProgressItem[] dto)
        => dto.Select(x => new KsbTaxonomyItem(x.Id, x.Value)).ToArray();
}

public record RecordApprenticeshipProgressResponse(
    long ProgressId
);