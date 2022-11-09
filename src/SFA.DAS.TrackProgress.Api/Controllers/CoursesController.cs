using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TrackProgress.Application.Commands;

namespace SFA.DAS.TrackProgress.Api.Controllers;

[ApiController]
[Route("/courses")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Route("ksbs")]
    public async Task<IActionResult> CacheKsbs(SaveKsbsCommand request)
    {
        await _mediator.Send(request);
        return Created("", null);
    }
}