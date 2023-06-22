using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ZDrive.Models;

public class Member
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ProjectId { get; set; } = default;

    [Required]
    [StringLength(10)]
    public string StudentNumber { get; set; } = string.Empty;

    public int Index { get; set; } = 0;

    [Required]
    public Role Role { get; set; } = default;

    public string? Description { get; set; } = string.Empty;

    [JsonIgnore]
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
