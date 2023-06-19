using System.Data.Common;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Extensions;
using ZDrive.Models;

namespace ZDrive_Test;

public class ProjectExtensionsTest
{
    [Test]
    public void Copy_DataOfCopiedProject_ShouldBeSameWithOriginal()
    {
        // Arrange
        var project1 = new TestDataBuilder<ProjectInformation>()
            .Randomize().Build();

        var project2 = new Project();

        // Act
        project2.Copy(project1);

        // Assert
        foreach (var property in project1.GetType().GetProperties())
        {
            if (property.Name == "Id" || property.Name == "UserId") continue;
            Assert.That(project2.GetType().GetProperty(property.Name)?.GetGetMethod()?.Invoke(project2, null), Is.EqualTo(property.GetGetMethod()?.Invoke(project1, null))
                , $"Failed in {property.ToString()}");
        }
    }

    [Test]
    public void Copy_HashCodeOfCopiedProject_ShouldNotBeChanged()
    {
        // Arrange
        var project1 = new ProjectInformation
        {
            Name = "Test",
            Description = "Test",
            EndDate = DateTime.Now,
            Status = Status.InProgress,
            FileSrc = "/Zdrive/build.zip"
        };

        var project2 = new Project();
        var id = project2.GetHashCode();

        // Act
        project2.Copy(project1);

        // Assert
        Assert.That(project2.GetHashCode(), Is.EqualTo(id));
    }
}