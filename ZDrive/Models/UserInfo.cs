using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ZDrive.Models;

public class UserInfo
{
    [StringLength(10)]
    public string StudentNumber { get; set; } = string.Empty;

    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    [StringLength(256)]
    public string Password { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public Authority Authority { get; set; } = Authority.User;
}