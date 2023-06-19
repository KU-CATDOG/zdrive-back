using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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

    [JsonIgnore]
    public ICollection<Member> Members { get; } = new List<Member>(); // Navigation collection

    [JsonIgnore]
    public User? User { get; set; }
}