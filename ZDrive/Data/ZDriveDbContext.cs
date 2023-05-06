using Microsoft.EntityFrameworkCore;
using ZDrive.Models;

namespace ZDrive.Data;

public class ZDriveDbContext : DbContext
{
    public ZDriveDbContext() : base() { }
    public ZDriveDbContext(DbContextOptions<ZDriveDbContext> options) : base(options) { }

    public virtual DbSet<User> Users => Set<User>();
    public virtual DbSet<Project> Projects => Set<Project>();
    public virtual DbSet<Image> Images => Set<Image>();
    public virtual DbSet<Milestone> Milestones => Set<Milestone>();
    public virtual DbSet<Member> Members => Set<Member>();
    public virtual DbSet<StudentNum> StudentNums => Set<StudentNum>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasMany(e => e.Images)
            .WithOne(e => e.Project)
            .HasForeignKey(e => e.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Project>()
            .HasMany(e => e.Members)
            .WithOne(e => e.Project)
            .HasForeignKey(e => e.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Project>()
            .HasMany(e => e.Milestones)
            .WithOne(e => e.Project)
            .HasForeignKey(e => e.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Member>()
            .HasOne(e => e.StudentNum)
            .WithOne(e => e.Member)
            .HasPrincipalKey<Member>(e => e.StudentNumber)
            .HasForeignKey<StudentNum>(e => e.StudentNumber)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<StudentNum>()
            .HasOne(e => e.User)
            .WithOne(e => e.StudentNum)
            .HasForeignKey<User>(e => e.StudentNumber)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}