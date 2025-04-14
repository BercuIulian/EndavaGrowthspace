using EndavaGrowthspace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EndavaGrowthspace.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.CreatedBy)
                .IsRequired();

            builder.Property(e => e.Contributors)
                .HasConversion(
                v => string.Join(',', v.Select(g => g.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => Guid.Parse(s))
                .ToList());

            builder.OwnsMany(c => c.Modules, cm =>
            {
                cm.WithOwner().HasForeignKey("CourseId");
                cm.Property<Guid>("CourseId");
                cm.HasKey("CourseId", "Id");
            });
        }
    }
}
