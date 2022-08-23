using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TrackProgress.Application.Commands.RecordApprenticeshipProgress;
using SFA.DAS.TrackProgress.DTOs;

namespace SFA.DAS.TrackProgress.Api.Controllers;

[ApiController]
[Route("/apprenticeships")]
public class TrackProgressController : ControllerBase
{
    private readonly IMediator mediator;

    public TrackProgressController(IMediator mediator) => this.mediator = mediator;

    [HttpPost("{apprenticeshipId}")]
    public async Task<IActionResult> AddProgress(long apprenticeshipId, ProgressDto progress)
    {
        await mediator.Send(new RecordApprenticeshipProgressCommand(apprenticeshipId, progress));
        return new StatusCodeResult(StatusCodes.Status201Created);
    }
}