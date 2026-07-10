using Microsoft.EntityFrameworkCore;
using CabScheduler.Api.Models;

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
    }
}
