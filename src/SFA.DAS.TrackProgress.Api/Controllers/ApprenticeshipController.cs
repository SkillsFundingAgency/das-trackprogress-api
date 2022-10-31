using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TrackProgress.Application.Commands;

namespace SFA.DAS.TrackProgress.Api.Controllers;

[ApiController]
[Route("/apprenticeships")]
public class ApprenticeshipController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApprenticeshipController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Route("{commitmentsApprenticeshipId}/snapshot")]
    public async Task<IActionResult> CreateSnapshot(long commitmentsApprenticeshipId)
    {
        await _mediator.Send(new CreateProgressSnapshotCommand(commitmentsApprenticeshipId));
        return Created("", null);
    }
}

[ApiController]
[Route("/courses")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Route("ksbs")]
    public async Task<IActionResult> CreateSnapshot(SaveKsbsCommand request)
    {
        await _mediator.Send(request);
        return Created("", null);
    }
}