using System.Security.Claims;

namespace ClinicAPI.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static int? GetPatientId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("patient_id")?.Value;
        return claim != null && int.TryParse(claim, out var id) ? id : null;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }
}
