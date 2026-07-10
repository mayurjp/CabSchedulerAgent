using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Models;
using RouteEntity = CabScheduler.Api.Models.Route;

namespace CabScheduler.Api.Data;

public class CabSchedulerDbContext : DbContext
{
    public CabSchedulerDbContext(DbContextOptions<CabSchedulerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Cab> Cabs => Set<Cab>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<CabRequest> CabRequests => Set<CabRequest>();
    public DbSet<RouteEntity> Routes => Set<RouteEntity>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasOne(d => d.AssignedCab)
                  .WithMany()
                  .HasForeignKey(d => d.CabId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CabRequest>(entity =>
        {
            entity.HasOne(cr => cr.Employee)
                  .WithMany()
                  .HasForeignKey(cr => cr.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RouteEntity>(entity =>
        {
            entity.HasOne(r => r.Cab)
                  .WithMany()
                  .HasForeignKey(r => r.CabId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Driver)
                  .WithMany()
                  .HasForeignKey(r => r.DriverId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasOne(a => a.Route)
                  .WithMany(r => r.Assignments)
                  .HasForeignKey(a => a.RouteId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Employee)
                  .WithMany()
                  .HasForeignKey(a => a.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.CabRequest)
                  .WithMany()
                  .HasForeignKey(a => a.CabRequestId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
