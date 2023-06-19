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

public class TestDataBuilder<T> where T : class, new()
{
    private T data;

    public TestDataBuilder()
    {
        data = new T();
    }

    public TestDataBuilder<T> Randomize()
    {
        var type = data.GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var value = GenerateRandomValue(property.PropertyType);
            property.GetSetMethod()?.Invoke(data, new object[] {value});
        }

        return this;
    }

    public TestDataBuilder<T> Setup(Action<T> setupExpression)
    {
        setupExpression?.Invoke(data);
        return this;
    }

    public T Build()
        => data;

    private object GenerateRandomValue(Type type)
    {
        if (type == typeof(int))
        {
            Random random = new Random();
            return random.Next(0, 100);
        }
        else if (type == typeof(string))
        {
            return GenerateRandomString(15);
        }
        else if (type == typeof(DateTime) || type == typeof(DateTime?))
        {
            Random random = new Random();
            return DateTime.Now.AddMinutes(random.Next(-1000000, 1000000));
        }
        else if (type.IsEnum)
        {
            Array values = Enum.GetValues(type);
            Random random = new Random();
            var randomEnum = values.GetValue(random.Next(values.Length));
            return randomEnum ?? null!;
        }
        else
        {
            return null!;
        }
    }

    private string GenerateRandomString(int length)
    {
        Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}