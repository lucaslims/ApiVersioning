using System.ComponentModel.DataAnnotations;

namespace ApiVersioning.Models;

public class AppUser
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public byte[] PasswordHash { get; set; }
    [Required]
    public byte[] PasswordSalt { get; set; }
}