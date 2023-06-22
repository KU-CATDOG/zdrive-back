using System.ComponentModel.DataAnnotations;

namespace ZDrive.Models;

public class ProjectInfo
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(3000)]
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Status Status { get; set; } = Status.NotStarted;

    public Visibility Visibility { get; set; } = Visibility.Public;

    [StringLength(100)]
    public string? Genre { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Engine { get; set; } = string.Empty;

    [StringLength(500)]
    public string? FileSrc { get; set; } = string.Empty;
}
