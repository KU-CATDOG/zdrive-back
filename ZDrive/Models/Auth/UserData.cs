namespace ZDrive.Models;

public record UserData
{
    public int Id;
    public string StudentNumber;
    public string Name;
    public Authority Authority;

    public UserData(User user)
    {
        this.Id = user.Id;
        this.StudentNumber = user.StudentNumber;
        this.Name = user.Name;
        this.Authority = user.Authority;
    }
}
