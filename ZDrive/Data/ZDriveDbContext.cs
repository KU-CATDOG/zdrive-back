using Microsoft.EntityFrameworkCore;
using ZDrive.Models;

namespace ZDrive.Data;

public class ZDriveDbContext : DbContext
{
    public ZDriveDbContext(DbContextOptions<ZDriveDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
}