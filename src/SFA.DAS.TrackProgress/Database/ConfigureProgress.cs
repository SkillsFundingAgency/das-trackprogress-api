using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Database;

public class ConfigureProgress : IEntityTypeConfiguration<Progress>
{
    public void Configure(EntityTypeBuilder<Progress> builder)
    {
        builder.HasKey(x => x.Id);
        builder.OwnsOne(progress => progress.ProviderApprenticeshipIdentifier, apprenticeship =>
        {
            apprenticeship.Property(p => p.ProviderId).HasColumnName(nameof(ProviderApprenticeshipIdentifier.ProviderId));
            apprenticeship.Property(p => p.Uln).HasColumnName(nameof(ProviderApprenticeshipIdentifier.Uln));
            apprenticeship.Property(p => p.StartDate)
                .HasColumnName(nameof(ProviderApprenticeshipIdentifier.StartDate))
                .HasColumnType("datetime");
        });
        builder.OwnsOne(progress => progress.Approval, approval =>
        {
            approval.Property(p => p.ApprenticeshipId).HasColumnName(nameof(ApprovalId.ApprenticeshipId));
            approval.Property(p => p.ApprenticeshipContinuationId).HasColumnName(nameof(ApprovalId.ApprenticeshipContinuationId));
        });
        builder.Property(x => x.ProgressData)
            .HasConversion<JsonValueConverter<KsbTaxonomy>>();
    }
}