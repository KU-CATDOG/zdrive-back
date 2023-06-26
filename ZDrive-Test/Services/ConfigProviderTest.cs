using ZDrive.Services;

namespace ZDrive_Test;

public class ConfigProviderTest
{
    private readonly string fileLocation = "../../../config.json";
    private ConfigProvider _config = null!;

    [Test]
    public void Getter_NonExistKey_ThrowsException()
    {
        // Arrange
        // Act
        // Assert
        Assert.Catch(() => { var ret = _config["empty"]; });
    }

    [Test]
    public void Getter_KeyOfStringValue_ReturnsValidValue()
    {
        // Arrange
        // Act
        var ret = _config["string"];
        // Assert
        Assert.That(ret, Is.EqualTo("test"));
    }

    [SetUp]
    public void SetUp()
    {
        _config = new ConfigProvider(fileLocation);
    }
}
