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
        modelBuilder.Entity<User>()
            .HasMany(e => e.Projects)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);

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

        modelBuilder.Entity<StudentNum>()
            .HasMany(e => e.Members)
            .WithOne(e => e.StudentNum)
            .HasForeignKey(e => e.StudentNumber)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentNum>()
            .HasOne(e => e.User)
            .WithOne(e => e.StudentNum)
            .HasForeignKey<User>(e => e.StudentNumber)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Member>()
            .Property(e => e.Role)
            .HasConversion(
                v => v.ToString(),
                v => (Role)Enum.Parse(typeof(Role), v)
            );

        modelBuilder.Entity<User>()
            .Property(e => e.Authority)
            .HasConversion(
                v => v.ToString(),
                v => (Authority)Enum.Parse(typeof(Authority), v)
            );

        modelBuilder.Entity<Project>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),
                v => (Status)Enum.Parse(typeof(Status), v)
            );

        modelBuilder.Entity<Project>()
            .Property(e => e.Visibility)
            .HasConversion(
                v => v.ToString(),
                v => (Visibility)Enum.Parse(typeof(Visibility), v)
            );
    }
}