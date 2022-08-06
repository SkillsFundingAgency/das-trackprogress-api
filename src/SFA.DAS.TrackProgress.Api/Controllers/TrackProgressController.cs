using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TrackProgress.Application;
using SFA.DAS.TrackProgress.DTOs;

namespace SFA.DAS.TrackProgressApi.Controllers;

[ApiController]
[Route("/apprenticeship")]
public class TrackProgressController : ControllerBase
{
    private readonly IMediator mediator;

    public TrackProgressController(IMediator mediator) => this.mediator = mediator;

    [HttpPost("{ukprn}/{uln}/{startDate}/progress")]
    public void Post(long ukprn, long uln, DateOnly startDate, ProgressDto progress)
    {
        mediator.Send(new RecordApprenticeshipProgress(ukprn, uln, startDate, progress));
    }
}
