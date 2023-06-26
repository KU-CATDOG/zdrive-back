using System.ComponentModel.DataAnnotations;

namespace ZDrive.Models;

public class MemberInfo
{
    [Required]
    [StringLength(10)]
    public string StudentNumber { get; set; } = string.Empty;

    public int Index { get; set; } = 0;

    [Required]
    public Role Role { get; set; } = default;

    public string? Description { get; set; } = string.Empty;
}
