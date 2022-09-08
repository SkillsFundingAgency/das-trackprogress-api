using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TrackProgress.Application.Commands.RecordApprenticeshipProgress;

namespace SFA.DAS.TrackProgress.Api.Controllers;

[ApiController]
[Route("/progress")]
public class TrackProgressController : ControllerBase
{
    private readonly IMediator mediator;

    public TrackProgressController(IMediator mediator) => this.mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> AddProgress(RecordApprenticeshipProgressCommand progress)
    {
        var response = await mediator.Send(progress);
        return Created("", response);
    }
}