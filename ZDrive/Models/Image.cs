using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ZDrive.Models;

public class Image
{
    [Key]
    [StringLength(500)]
    public string ImageSrc { get; set; } = string.Empty;

    [Required]
    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;
}