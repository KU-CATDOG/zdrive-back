using System.ComponentModel.DataAnnotations;

namespace ZDrive.Models;

public class StudentNumInfo
{
    [Required]
    [StringLength(10)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
