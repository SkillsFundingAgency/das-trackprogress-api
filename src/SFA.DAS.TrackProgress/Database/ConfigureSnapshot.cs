using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Database;

public class ConfigureSnapshot : IEntityTypeConfiguration<Snapshot>
{
    public void Configure(EntityTypeBuilder<Snapshot> builder)
    {
        builder.HasKey(x => x.Id);
        builder.OwnsOne(progress => progress.Approval, approval =>
        {
            approval.Property(p => p.ApprenticeshipId).HasColumnName("CommitmentsApprenticeshipId");
            approval.Property(p => p.ApprenticeshipContinuationId).HasColumnName("CommitmentsContinuationId");
        });
    }
}