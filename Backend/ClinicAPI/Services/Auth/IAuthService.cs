using ClinicAPI.Models.Dtos.Auth;

namespace ClinicAPI.Services.Auth;

public interface IAuthService
{
    public Task<TokenResponseDto?> Login(LoginRequestDto request);
    public Task<bool> IsEmailTaken(string email);
    public Task<bool> IsEmailTakenByAnotherPatient(string email, int patientId);
}
