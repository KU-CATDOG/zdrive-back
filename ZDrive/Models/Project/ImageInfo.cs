using System.ComponentModel.DataAnnotations;

namespace ZDrive.Models;

public class ImageInfo
{
    [Required]
    [StringLength(500)]
    public string ImageSrc { get; set; } = string.Empty;

    public int Index { get; set; } = 0;
}