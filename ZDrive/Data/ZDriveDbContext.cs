using Microsoft.EntityFrameworkCore;
using ZDrive.Models;

namespace ZDrive.Data;

public class ZDriveDbContext : DbContext
{
    public ZDriveDbContext() : base() { }
    public ZDriveDbContext(DbContextOptions<ZDriveDbContext> options) : base(options) { }

    public virtual DbSet<User> Users => Set<User>();
}