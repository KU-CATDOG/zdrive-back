using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZDrive.Models;

public class StudentNum
{
    [Key]
    [Required]
    [StringLength(10)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public Member Member { get; set; } = null!;

    public User? User { get; set; }
}