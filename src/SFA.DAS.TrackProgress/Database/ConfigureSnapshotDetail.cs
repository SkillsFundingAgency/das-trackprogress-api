using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Database;

public class ConfigureSnapshotDetail : IEntityTypeConfiguration<SnapshotDetail>
{
    public void Configure(EntityTypeBuilder<SnapshotDetail> builder)
    {
        builder.Property<long>("Id");
        builder.HasKey("Id");
    }
}
