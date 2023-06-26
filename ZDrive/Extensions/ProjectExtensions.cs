using ZDrive.Models;

namespace ZDrive.Extensions;

public static class ProjectExtensions
{
    public static void Copy(this Project _project, ProjectInfo project)
    {
        _project.Name = project.Name;
        _project.Description = project.Description;
        _project.StartDate = project.StartDate;
        _project.EndDate = project.EndDate;
        _project.Visibility = project.Visibility;
        _project.Status = project.Status;
        _project.Genre = project.Genre;
        _project.Engine = project.Engine;
        _project.FileSrc = project.FileSrc;
    }
}
