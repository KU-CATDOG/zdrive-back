using System.Collections;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Models;

namespace ZDrive_Test;

public class TestDbContextCreater : IDisposable
{
    private readonly DbConnection _connection = null!;
    private readonly DbContextOptions<ZDriveDbContext> _contextOptions = null!;

    public ZDriveDbContext Create() => new ZDriveDbContext(_contextOptions);

    public TestDbContextCreater()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ZDriveDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new ZDriveDbContext(_contextOptions);

        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    public void Setup(Action<ZDriveDbContext> expression)
    {
        using var context = new ZDriveDbContext(_contextOptions);
        expression(context);
    }

    public void Dispose() => _connection.Dispose();
}