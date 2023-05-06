using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZDrive.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string StudentNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(32)]
    public string Salt { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public int Authority { get; set; } = 0;

    public StudentNum StudentNum { get; set; } = null!;
}