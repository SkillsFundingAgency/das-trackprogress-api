using MediatR;
using SFA.DAS.TrackProgress.DTOs;

namespace SFA.DAS.TrackProgress.Application;

public record ApprenticeshipId { long Ukrpn; long Uln; DateOnly StartDate; };

public class RecordApprenticeshipProgress : IRequest
{
    public RecordApprenticeshipProgress(long ukprn, long uln, DateOnly startDate, ProgressDto progress)
    {
        Ukprn = ukprn;
        Uln = uln;
        StartDate = startDate;
        Progress = progress;
    }

    public long Ukprn { get; set; }
    public long Uln { get; set; }
    public DateOnly StartDate { get; set; }
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
        context.Progress.Add(new Models.Progress(request.Ukprn, request.Uln, request.StartDate, request.Progress));
        await context.SaveChangesAsync();
        return Unit.Value;
    }
}