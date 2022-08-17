using MediatR;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.DTOs;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application;

public class RecordApprenticeshipProgress : IRequest
{
    public RecordApprenticeshipProgress(ApprenticeshipId apprenticeship, KsbProgress progress)
    {
        Apprenticeship = apprenticeship;
        Progress = progress;
    }

    public ApprenticeshipId Apprenticeship { get; set; }
    public KsbProgress Progress { get; set; }
}

public class RecordApprenticeshipProgressHandler : IRequestHandler<RecordApprenticeshipProgress>
{
    private readonly TrackProgressContext context;

    public RecordApprenticeshipProgressHandler(TrackProgressContext context)
        => this.context = context;

    public async Task<Unit> Handle(
        RecordApprenticeshipProgress request, CancellationToken cancellationToken)
    {
        context.Progress.Add(
            new Progress(
                request.Apprenticeship,
                request.Progress.ApprovalId,
                new KsbTaxonomy(
                    ToDomainTaxonomy(request.Progress.Knowledges),
                    ToDomainTaxonomy(request.Progress.Skills),
                    ToDomainTaxonomy(request.Progress.Behaviours))));

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static KsbTaxonomyItem[] ToDomainTaxonomy(ProgressItem[] dto)
        => dto.Select(x => new KsbTaxonomyItem(x.Id, x.Value)).ToArray();
}