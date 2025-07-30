using CloudWorks.Application.Common.Interfaces;
using CloudWorks.Data.Entities;
using CloudWorks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CloudWorks.IntegrationTests.InMemoryPersistence;


public class TestDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<AccessPoint> AccessPoints { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<SiteProfile> SiteProfiles { get; set; }
    public DbSet<AccessPointHistory> AccessPointHistories { get; set; }
    public DbSet<BookingAssessPoint> BookingAssessPoints { get; set; }
    public DbSet<BookingSiteProfile> BookingSiteProfiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CloudWorksDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}