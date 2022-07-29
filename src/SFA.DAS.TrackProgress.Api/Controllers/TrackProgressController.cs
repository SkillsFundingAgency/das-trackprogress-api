using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.TrackProgressApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TrackProgressController : ControllerBase
{
    [HttpGet(Name = "TrackProgress")]
    public void Get()
    {
    }
}