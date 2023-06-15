using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZDrive.Models;

public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

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

    public User User { get; set; } = null!;

    public ICollection<Image> Images { get; set; } = new List<Image>(); // Navigation collection
    public ICollection<Member> Members { get; set; } = new List<Member>(); // Navigation collection
    public ICollection<Milestone> Milestones { get; } = new List<Milestone>(); // Navigation collection
}

public enum Status
{
    NotStarted,
    InProgress,
    Completed
}

public enum Visibility
{
    Public,
    Private
}