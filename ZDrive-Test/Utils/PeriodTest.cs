using ZDrive.Utils;

namespace ZDrive_Test;

public class PeriodTest
{
    [Test]
    public void Constructor_ValidString_ShouldBeCreatePeriod()
    {
        // Arrange
        var str = "2023-1";

        // Act
        var ret = new Period(str);

        // Assert
        Assert.That(ret.Year, Is.EqualTo(2023));
        Assert.That(ret.Semester, Is.EqualTo(Semester.First));
    }

    [Test]
    public void Constructor_InvalidString_ThrowsFormatException()
    {
        // Arrange
        var str = "2023-1awhfeilaw";

        // Act

        // Assert
        Assert.Catch(() => new Period(str));
    }

    [Test]
    public void IsWithinPeriod_DateTimeInPeriod_ReturnsTrue()
    {
        // Arrange
        var date = DateTime.Parse("04/10/2023");
        var period = new Period(2023, Semester.First);

        // Act
        var ret = period.IsWithInPeriod(date);

        // Assert
        Assert.That(ret, Is.True);
    }

    [Test]
    public void IsWithinPeriod_DateTimeNotInPeriod_ReturnsFalse()
    {
        // Arrange
        var date = DateTime.Parse("02/10/2021");
        var period = new Period(2023, Semester.First);

        // Act
        var ret = period.IsWithInPeriod(date);

        // Assert
        Assert.That(ret, Is.False);
    }
}