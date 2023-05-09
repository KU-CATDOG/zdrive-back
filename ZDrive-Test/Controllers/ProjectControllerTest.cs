using Moq;
using ZDrive.Controllers;
using ZDrive.Data;
using ZDrive.Services;

namespace ZDrive_Test;

public class ProjectControllerTest
{
    private Mock<ZDriveDbContext> mockContext = null!;
    private Mock<IAuthorizationManager> mockAuth = null!;

    [SetUp]
    public void SetUp()
    {

    }

    private ProjectController CreateController()
        => new ProjectController(mockAuth.Object, mockContext.Object);
}