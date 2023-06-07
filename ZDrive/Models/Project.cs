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

    [StringLength(20)]
    public string? Status { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public ICollection<Image> Images { get; } = new List<Image>(); // Navigation collection
    public ICollection<Member> Members { get; } = new List<Member>(); // Navigation collection
    public ICollection<Milestone> Milestones { get; } = new List<Milestone>(); // Navigation collection
}