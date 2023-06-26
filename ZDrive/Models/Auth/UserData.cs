namespace ZDrive.Models;

public record UserData
{
    public int Id { get; init; }
    public string StudentNumber { get; init; }
    public string Name { get; init; }
    public Authority Authority { get; init; }

    public UserData(User user)
    {
        this.Id = user.Id;
        this.StudentNumber = user.StudentNumber;
        this.Name = user.Name;
        this.Authority = user.Authority;
    }

    public static UserData User(User user)
        => new UserData(user);
}
