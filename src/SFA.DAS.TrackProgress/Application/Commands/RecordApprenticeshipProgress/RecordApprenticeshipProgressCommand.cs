using MediatR;
using SFA.DAS.TrackProgress.DTOs;

namespace SFA.DAS.TrackProgress.Application.Commands.RecordApprenticeshipProgress;

public class RecordApprenticeshipProgressCommand : IRequest
{
    public RecordApprenticeshipProgressCommand(long apprenticeshipId, ProgressDto progress)
    {
        ApprenticeshipId = apprenticeshipId;
        Progress = progress;
    }

    public long ApprenticeshipId { get; set; }
    public ProgressDto Progress { get; set; }
}