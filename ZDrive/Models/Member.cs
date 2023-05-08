using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZDrive.Models;

[PrimaryKey(nameof(StudentNumber), nameof(Role))]
public class Member
{
    [StringLength(10)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } = default;

    [Required]
    public int ProjectId { get; set; } = default;

    public Project Project { get; set; } = null!;

    public StudentNum StudentNum { get; set; } = null!;
}

public enum Role
{
    Programmer,
    GraphicArtist,
    SoundArtist,
    GameDesigner,
    ProjectManager
}