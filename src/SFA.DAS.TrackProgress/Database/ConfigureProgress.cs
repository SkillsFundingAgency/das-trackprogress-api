using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Database;

public class ConfigureProgress : IEntityTypeConfiguration<Progress>
{
    public void Configure(EntityTypeBuilder<Progress> builder)
    {
        builder.HasKey(x => x.Id);
        builder.OwnsOne(progress => progress.Apprenticeship, apprenticeship =>
        {
            apprenticeship.Property(p => p.Ukprn).HasColumnName(nameof(ApprenticeshipId.Ukprn));
            apprenticeship.Property(p => p.Uln).HasColumnName(nameof(ApprenticeshipId.Uln));
            apprenticeship.Property(p => p.StartDate)
                .HasColumnName(nameof(ApprenticeshipId.StartDate))
                .HasConversion<DateOnlyConverter>()
                .HasColumnType("date");
        });
        builder.OwnsOne(progress => progress.Approval, approval =>
        {
            approval.Property(p => p.ApprenticeshipId).HasColumnName("ApprovalId");
            approval.Property(p => p.ContinuationId).HasColumnName("ApprovalContinuationId");
        });
        builder.Property(x => x.ProgressData)
            .HasConversion<JsonValueConverter<KsbTaxonomy>>();
    }
}