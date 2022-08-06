using Microsoft.EntityFrameworkCore;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress;

public class TrackProgressContext : DbContext
{
    public TrackProgressContext(DbContextOptions<TrackProgressContext> options) : base(options)
    {
    }

    public DbSet<Progress> Progress { get; set; } = null!;
}
