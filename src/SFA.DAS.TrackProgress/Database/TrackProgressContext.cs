using Microsoft.EntityFrameworkCore;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Database;

public class TrackProgressContext : DbContext
{
    public TrackProgressContext(DbContextOptions<TrackProgressContext> options) : base(options)
    {
    }

    public DbSet<Progress> Progress { get; set; } = null!;
    public DbSet<Snapshot> Snapshot { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ConfigureProgress());
        modelBuilder.ApplyConfiguration(new ConfigureSnapshot());
    }
}
