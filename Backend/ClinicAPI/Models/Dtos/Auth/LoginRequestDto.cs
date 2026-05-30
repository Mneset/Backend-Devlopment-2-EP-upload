using System.ComponentModel.DataAnnotations;

namespace ClinicAPI.Models.Dtos.Auth;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(1)]
    public string Password { get; set; }
}
