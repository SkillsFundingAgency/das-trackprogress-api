using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Database;

public class ConfigureKsbName : IEntityTypeConfiguration<KsbName>
{
    public void Configure(EntityTypeBuilder<KsbName> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
