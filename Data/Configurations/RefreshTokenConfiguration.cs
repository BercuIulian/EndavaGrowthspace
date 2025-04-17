using EndavaGrowthspace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EndavaGrowthspace.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Token);

            builder.Property(rt => rt.JwtId)
                .IsRequired();

            builder.Property(rt => rt.CreationDate)
                .IsRequired();

            builder.Property(rt => rt.ExpiryDate)
                .IsRequired();
        }
    }
}
