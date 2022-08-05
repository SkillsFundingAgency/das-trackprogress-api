using MediatR;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.DTOs;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Application;

public class RecordApprenticeshipProgress : IRequest
{
    public RecordApprenticeshipProgress(ApprenticeshipId apprenticeship, ProgressDto progress)
    {
        Apprenticeship = apprenticeship;
        Progress = progress;
    }

    public ApprenticeshipId Apprenticeship { get; set; }
    public ProgressDto Progress { get; set; }
}

public class RecordApprenticeshipProgressHandler : IRequestHandler<RecordApprenticeshipProgress>
{
    private readonly TrackProgressContext context;

    public RecordApprenticeshipProgressHandler(TrackProgressContext context)
    {
        this.context = context;
    }

    public async Task<Unit> Handle(RecordApprenticeshipProgress request, CancellationToken cancellationToken)
    {
        context.Progress.Add(new Models.Progress(request.Apprenticeship, request.Progress));
        await context.SaveChangesAsync();
        return Unit.Value;
    }
}