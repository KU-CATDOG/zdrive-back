using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Models;

namespace ZDrive_Test;

public class TestDbContext
{
    private readonly Mock<ZDriveDbContext> _inst = new Mock<ZDriveDbContext>();
    private readonly List<User> users = new List<User>();
    private readonly List<Project> projects = new List<Project>();
    private readonly List<Member> members = new List<Member>();
    private readonly List<Image> images = new List<Image>();
    private readonly List<Milestone> milestones = new List<Milestone>();
    private readonly List<StudentNum> studentNums = new List<StudentNum>();

    public List<User> Users => users;
    public List<Project> Projects => projects;
    public List<Member> Members => members;
    public List<Image> Images => images;
    public List<Milestone> Milestones => milestones;
    public List<StudentNum> StudentNums => studentNums;

    public ZDriveDbContext Object => _inst.Object;

    public void Setup(Action<Mock<ZDriveDbContext>>? expression = null)
    {
        _inst.Setup(x => x.Users).ReturnsDbSet(users);
        _inst.Setup(x => x.Projects).ReturnsDbSet(projects);
        _inst.Setup(x => x.Members).ReturnsDbSet(members);
        _inst.Setup(x => x.Images).ReturnsDbSet(images);
        _inst.Setup(x => x.Milestones).ReturnsDbSet(milestones);
        _inst.Setup(x => x.StudentNums).ReturnsDbSet(studentNums);

        _inst.Setup(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<System.Threading.CancellationToken>()))
            .Callback((User user, CancellationToken token) => { users.Add(user); });
        _inst.Setup(x => x.Projects.AddAsync(It.IsAny<Project>(), It.IsAny<System.Threading.CancellationToken>()))
            .Callback((Project project, CancellationToken token) => { projects.Add(project); });
        _inst.Setup(x => x.Members.AddAsync(It.IsAny<Member>(), It.IsAny<System.Threading.CancellationToken>()))
            .Callback((Member member, CancellationToken token) => { members.Add(member); });
        _inst.Setup(x => x.Images.AddAsync(It.IsAny<Image>(), It.IsAny<System.Threading.CancellationToken>()))
            .Callback((Image image, CancellationToken token) => { images.Add(image); });
        _inst.Setup(x => x.Milestones.AddAsync(It.IsAny<Milestone>(), It.IsAny<System.Threading.CancellationToken>()))
            .Callback((Milestone milestone, CancellationToken token) => { milestones.Add(milestone); });
        _inst.Setup(x => x.StudentNums.AddAsync(It.IsAny<StudentNum>(), It.IsAny<System.Threading.CancellationToken>()))
            .Callback((StudentNum studentNum, CancellationToken token) => { studentNums.Add(studentNum); });

        _inst.Setup(x => x.Users.Remove(It.IsAny<User>()))
            .Callback((User user) => { users.Remove(user); });
        _inst.Setup(x => x.Projects.Remove(It.IsAny<Project>()))
            .Callback((Project project) => { projects.Remove(project); });
        _inst.Setup(x => x.Images.Remove(It.IsAny<Image>()))
            .Callback((Image image) => { images.Remove(image); });
        _inst.Setup(x => x.Members.Remove(It.IsAny<Member>()))
            .Callback((Member member) => { members.Remove(member); });
        _inst.Setup(x => x.Milestones.Remove(It.IsAny<Milestone>()))
            .Callback((Milestone milestone) => { milestones.Remove(milestone); });
        _inst.Setup(x => x.StudentNums.Remove(It.IsAny<StudentNum>()))
            .Callback((StudentNum studentNum) => { studentNums.Remove(studentNum); });

        _inst.Setup(x => x.SaveChangesAsync(default))
            .Returns(Task.FromResult(1))
            .Verifiable();

        expression?.Invoke(_inst);
    }

    public void AddElementsInUserTable(IEnumerable<User> elements)
    {
        foreach (var element in elements)
            users.Add(element);
    }

    public void AddElementsInProjectTable(IEnumerable<Project> elements)
    {
        foreach (var element in elements)
            projects.Add(element);
    }

    public void AddElementsInMemberTable(IEnumerable<Member> elements)
    {
        foreach (var element in elements)
            members.Add(element);
    }

    public void AddElementsInImageTable(IEnumerable<Image> elements)
    {
        foreach (var element in elements)
            images.Add(element);
    }

    public void AddElementsInMilestoneTable(IEnumerable<Milestone> elements)
    {
        foreach (var element in elements)
            milestones.Add(element);
    }

    public void AddElementsInStudentNumTable(IEnumerable<StudentNum> elements)
    {
        foreach (var element in elements)
            studentNums.Add(element);
    }

    public void Verify() => _inst.Verify();
}