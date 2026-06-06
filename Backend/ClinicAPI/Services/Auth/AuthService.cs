using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicAPI.Data;
using ClinicAPI.Models.Config;
using ClinicAPI.Models.Dtos.Auth;
using ClinicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClinicAPI.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ClinicDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(ClinicDbContext context, JwtSettings jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings;
    }

    public async Task<bool> IsEmailTaken(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsEmailTakenByAnotherPatient(string email, int patientId)
    {
        return await _context.Users.AnyAsync(u =>
            u.Email == email &&
            (u.PatientId == null || u.PatientId != patientId));
    }

    public async Task<TokenResponseDto?> Login(LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.Patient)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || user.PasswordHash == null)
        {
            return null;
        }

        if (user.Patient != null && !user.Patient.IsActive)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);
        var token = GenerateToken(user, expiresAt);

        return new TokenResponseDto
        {
            Token = token,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    private string GenerateToken(User user, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.PatientId.HasValue)
        {
            claims.Add(new Claim("patient_id", user.PatientId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
