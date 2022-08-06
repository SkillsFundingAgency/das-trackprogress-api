using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TrackProgress;

namespace SFA.DAS.TrackProgressApi.Controllers;

[ApiController]
[Route("/apprenticeship")]
public class TrackProgressController : ControllerBase
{
    public TrackProgressController(TrackProgressContext context)
    {
        Context = context;
    }

    public TrackProgressContext Context { get; }

    [HttpPost("{ukprn}/{uln}/{startDate}/progress")]
    public void Post(long ukprn, long uln, DateOnly startDate)
    {
        Context.Progress.Add(new TrackProgress.Models.Progress());
        Context.SaveChanges();
    }
}
