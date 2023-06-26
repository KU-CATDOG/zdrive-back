using ZDrive.Models;

namespace ZDrive_Test;

public class UserDataTest
{
    private User CreateRandomUser()
        => new TestDataBuilder<User>().Randomize().Build();

    [Test]
    public void Equals_SameUserData_ReturnsTrue()
    {
        // Arrange
        var user = CreateRandomUser();
        var data1 = UserData.User(user);
        var data2 = UserData.User(user);

        // Act
        var ret1 = data1 == data2;
        var ret2 = data1.Equals(data2);

        // Assert
        Assert.That(ret1, Is.True);
        Assert.That(ret2, Is.True);
    }

    [Test]
    public void ReferenceEquals_DifferentRecord_ReturnsFalse()
    {
        // Arrange
        var user = CreateRandomUser();
        var data1 = UserData.User(user);
        var data2 = UserData.User(user);

        // Act
        var ret = ReferenceEquals(data1, data2);

        // Assert
        Assert.That(ret, Is.False);
    }
}
