using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TrackProgress.Application.Commands;

namespace SFA.DAS.TrackProgress.Api.Controllers;

[ApiController]
[Route("/apprenticeship")]
public class ApprenticeshipController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApprenticeshipController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Route("{commitmentsApprenticeshipId}/snapshot")]
    public async Task<IActionResult> CreateSnapshot(long commitmentsApprenticeshipId)
    {
        await _mediator.Send(new CreateProgressSnapshotCommand(commitmentsApprenticeshipId));
        return Ok();
    }
}
