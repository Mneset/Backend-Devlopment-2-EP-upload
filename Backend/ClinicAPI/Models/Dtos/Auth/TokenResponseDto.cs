using ClinicAPI.Models.Entities;

namespace ClinicAPI.Models.Dtos.Auth;

public class TokenResponseDto
{
    public string Token { get; set; }
    public Role Role { get; set; }
    public DateTime ExpiresAt { get; set; }
}
