using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TrackProgress.Application;
using SFA.DAS.TrackProgress.DTOs;
using SFA.DAS.TrackProgress.Models;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.TrackProgressApi.Controllers;

[ApiController]
[Route("/apprenticeship")]
public class TrackProgressController : ControllerBase
{
    private readonly IMediator mediator;

    public TrackProgressController(IMediator mediator) => this.mediator = mediator;

    [HttpPost("{ukprn}/{uln}/{startDate}/progress")]
    public async Task AddProgress(
        [Range(1, long.MaxValue, ErrorMessage = "UKPRN must be greater than zero.")] long ukprn,
        [Range(1, long.MaxValue, ErrorMessage = "ULN must be greater than zero.")] long uln,
        DateOnly startDate, KsbProgress progress)
    {
        await mediator.Send(new RecordApprenticeshipProgress(new ApprenticeshipId(ukprn, uln, startDate), progress));
    }
}
