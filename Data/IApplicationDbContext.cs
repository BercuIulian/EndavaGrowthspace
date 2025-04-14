using EndavaGrowthspace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EndavaGrowthspace.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Course> Courses { get; set; }
        DbSet<Module> Modules { get; set; }
        DbSet<User> Users { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}
