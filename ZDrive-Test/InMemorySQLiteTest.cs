using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Models;

namespace ZDrive_Test;

public class InMemorySQLiteTest
{
    private DbConnection _connection = null!;
    private DbContextOptions<ZDriveDbContext> _contextOptions = null!;

    private ZDriveDbContext CreateContext() => new ZDriveDbContext(_contextOptions);

    [SetUp]
    public void Setup()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ZDriveDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new ZDriveDbContext(_contextOptions);

        context.Database.EnsureCreated();

        context.StudentNums.AddRange(
            new StudentNum { StudentNumber = "2021320006", Name = "Minjong" },
            new StudentNum { StudentNumber = "2020320124", Name = "Chaenamul" }
        );
        context.SaveChanges();
    }

    [Test]
    public void SqliteInMemoryTest()
    {
        using var context = CreateContext();

        var stdNum = context.StudentNums.Find("2021320006");

        Assert.That(stdNum?.StudentNumber, Is.EqualTo("2021320006"));
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Dispose();
    }
}
