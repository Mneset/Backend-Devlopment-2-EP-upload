using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicAPI.Models.Dtos.Appointment;
using ClinicAPI.Models.Dtos.Auth;
using ClinicAPI.Models.Dtos.Patient;
using ClinicAPI.Models.Entities;
using FluentAssertions;

namespace ClinicAPI.Tests;

public class PatientControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public PatientControllerTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task LoginAsAdminAsync()
    {
        await LoginAsAsync("admin@clinic.com", "Admin123");
    }

    private async Task LoginAsPatientAsync()
    {
        await LoginAsAsync("jonas@email.com", "Patient123");
    }

    private async Task LoginAsAsync(string email, string password)
    {
        var loginRequest = new LoginRequestDto { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var body = await response.Content.ReadFromJsonAsync<TokenResponseDto>(TestWebAppFactory.JsonOptions);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    [Fact]
    public async Task GetAllPatients_AsAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/patients");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllPatients_AsPatient_ReturnsForbidden()
    {
        await LoginAsPatientAsync();

        var response = await _client.GetAsync("/api/v1/patients");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllPatients_AsAdmin_ReturnsActiveSeededPatients()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/patients");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var patients = await response.Content.ReadFromJsonAsync<List<PatientResponseDto>>(TestWebAppFactory.JsonOptions);
        patients.Should().HaveCount(3);
        patients.Should().OnlyContain(p => p.IsActive);
    }

    [Fact]
    public async Task GetPatientById_AsAdmin_WithValidId_ReturnsPatient()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/patients/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var patient = await response.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);
        patient!.Id.Should().Be(1);
        patient.FirstName.Should().Be("Jonas");
        patient.LastName.Should().Be("Nilsen");
        patient.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetPatientById_AsAdmin_WithInactiveId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/patients/4");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPatientById_AsAdmin_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/patients/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllPatientsIncludingInactive_AsAdmin_ReturnsAllPatients()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/patients/all");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var patients = await response.Content.ReadFromJsonAsync<List<PatientResponseDto>>(TestWebAppFactory.JsonOptions);
        patients.Should().HaveCount(4);
        patients.Should().Contain(p => p.Id == 4 && !p.IsActive);
    }

    [Fact]
    public async Task GetPatientByIdIncludingInactive_AsAdmin_ReturnsInactivePatient()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/patients/all/4");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var patient = await response.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);
        patient!.Id.Should().Be(4);
        patient.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetPatientByIdIncludingInactive_AsAdmin_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/patients/all/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePatient_AsAdmin_UpdatesFields()
    {
        await LoginAsAdminAsync();

        var request = new UpdatePatientRequestDto
        {
            Id = 1,
            Email = "jonas@email.com",
            FirstName = "UpdatedJonas",
            LastName = "Nilsen",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/patients/1");
        var updated = await verify.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);
        updated!.FirstName.Should().Be("UpdatedJonas");
    }

    [Fact]
    public async Task UpdatePatient_WithIdMismatch_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new UpdatePatientRequestDto
        {
            Id = 2,
            Email = "marie@email.com",
            FirstName = "Mismatched",
            LastName = "User",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePatient_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var request = new UpdatePatientRequestDto
        {
            Id = 9999,
            Email = "ghost@test.com",
            FirstName = "Doesnt",
            LastName = "Exist",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/9999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePatient_AsAdmin_WithEmailTakenByAnotherPatient_ReturnsConflict()
    {
        await LoginAsAdminAsync();

        var request = new UpdatePatientRequestDto
        {
            Id = 2,
            Email = "jonas@email.com",
            FirstName = "Marie",
            LastName = "Andersen",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1985, 3, 22)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/2", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetMyProfile_AsAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/patients/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyProfile_AsAdmin_ReturnsForbidden()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/patients/me");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetMyProfile_AsPatient_ReturnsProfile()
    {
        await LoginAsPatientAsync();

        var response = await _client.GetAsync("/api/v1/patients/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var profile = await response.Content.ReadFromJsonAsync<MyProfileResponseDto>(TestWebAppFactory.JsonOptions);
        profile!.Id.Should().Be(1);
        profile.Email.Should().Be("jonas@email.com");
        profile.FirstName.Should().Be("Jonas");
        profile.LastName.Should().Be("Nilsen");
    }

    [Fact]
    public async Task GetMyProfile_AsDeactivatedPatientWithStaleToken_ReturnsForbidden()
    {
        var register = new RegisterPatientRequestDto
        {
            Email = "stalegetprofile@test.com",
            Password = "Patient123",
            FirstName = "Stale",
            LastName = "Get",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            Religion = Religion.None,
            SocialSecurityNumber = "33344455500",
            TaxNumber = "TX333",
            DriverLicenseNumber = "DL333",
            MedicalInsuranceNumber = "MI333"
        };
        var registerResp = await _client.PostAsJsonAsync("/api/v1/auth/register/patient", register);
        var created = await registerResp.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);

        await LoginAsAsync("stalegetprofile@test.com", "Patient123");
        var staleAuth = _client.DefaultRequestHeaders.Authorization;

        await LoginAsAdminAsync();
        var deactivate = await _client.DeleteAsync($"/api/v1/patients/{created!.Id}");
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Authorization = staleAuth;
        var response = await _client.GetAsync("/api/v1/patients/me");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateMyProfile_AsAnonymous_ReturnsUnauthorized()
    {
        var request = new UpdateOwnProfileRequestDto
        {
            Email = "jonas@email.com",
            FirstName = "Jonas",
            LastName = "Nilsen",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/me", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMyProfile_AsAdmin_ReturnsForbidden()
    {
        await LoginAsAdminAsync();

        var request = new UpdateOwnProfileRequestDto
        {
            Email = "jonas@email.com",
            FirstName = "Jonas",
            LastName = "Nilsen",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/me", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateMyProfile_AsPatient_UpdatesOwnFieldsAndEmail()
    {
        var register = new RegisterPatientRequestDto
        {
            Email = "selfupdate@test.com",
            Password = "Patient123",
            FirstName = "Self",
            LastName = "Update",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1992, 1, 1),
            Religion = Religion.None,
            SocialSecurityNumber = "99900011100",
            TaxNumber = "TX999",
            DriverLicenseNumber = "DL999",
            MedicalInsuranceNumber = "MI999"
        };
        var registerResp = await _client.PostAsJsonAsync("/api/v1/auth/register/patient", register);
        var created = await registerResp.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);

        await LoginAsAsync("selfupdate@test.com", "Patient123");

        var request = new UpdateOwnProfileRequestDto
        {
            Email = "selfupdate-new@test.com",
            FirstName = "SelfUpdated",
            LastName = "UpdateNew",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1992, 1, 1),
            Religion = Religion.Buddhism
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/me", request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await LoginAsAdminAsync();
        var verify = await _client.GetAsync($"/api/v1/patients/{created!.Id}");
        var updated = await verify.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);
        updated!.FirstName.Should().Be("SelfUpdated");
        updated.LastName.Should().Be("UpdateNew");
        updated.Religion.Should().Be(Religion.Buddhism);
    }

    [Fact]
    public async Task UpdateMyProfile_AsPatient_WithSameEmail_Succeeds()
    {
        await LoginAsPatientAsync();

        var request = new UpdateOwnProfileRequestDto
        {
            Email = "jonas@email.com",
            FirstName = "Jonas",
            LastName = "Nilsen",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/me", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateMyProfile_AsPatient_WithEmailTakenByAnother_ReturnsConflict()
    {
        await LoginAsPatientAsync();

        var request = new UpdateOwnProfileRequestDto
        {
            Email = "admin@clinic.com",
            FirstName = "Jonas",
            LastName = "Nilsen",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/me", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateMyProfile_AsDeactivatedPatientWithStaleToken_ReturnsForbidden()
    {
        var register = new RegisterPatientRequestDto
        {
            Email = "staleprofile@test.com",
            Password = "Patient123",
            FirstName = "Stale",
            LastName = "Profile",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            Religion = Religion.None,
            SocialSecurityNumber = "11122233300",
            TaxNumber = "TX111",
            DriverLicenseNumber = "DL111",
            MedicalInsuranceNumber = "MI111"
        };
        var registerResp = await _client.PostAsJsonAsync("/api/v1/auth/register/patient", register);
        var created = await registerResp.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);

        await LoginAsAsync("staleprofile@test.com", "Patient123");
        var staleAuth = _client.DefaultRequestHeaders.Authorization;

        await LoginAsAdminAsync();
        var deactivate = await _client.DeleteAsync($"/api/v1/patients/{created!.Id}");
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Authorization = staleAuth;
        var request = new UpdateOwnProfileRequestDto
        {
            Email = "staleprofile@test.com",
            FirstName = "Stale",
            LastName = "Profile",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var response = await _client.PutAsJsonAsync("/api/v1/patients/me", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateMyProfile_NewEmailWorksForLogin()
    {
        var register = new RegisterPatientRequestDto
        {
            Email = "loginafterupdate@test.com",
            Password = "Patient123",
            FirstName = "Login",
            LastName = "AfterUpdate",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            Religion = Religion.None,
            SocialSecurityNumber = "44455566600",
            TaxNumber = "TX444",
            DriverLicenseNumber = "DL444",
            MedicalInsuranceNumber = "MI444"
        };
        await _client.PostAsJsonAsync("/api/v1/auth/register/patient", register);

        await LoginAsAsync("loginafterupdate@test.com", "Patient123");

        var newEmail = "loginafterupdate-new@test.com";
        var update = new UpdateOwnProfileRequestDto
        {
            Email = newEmail,
            FirstName = "Login",
            LastName = "AfterUpdate",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var updateResp = await _client.PutAsJsonAsync("/api/v1/patients/me", update);
        updateResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Authorization = null;
        var loginRequest = new LoginRequestDto { Email = newEmail, Password = "Patient123" };
        var loginResp = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivatePatient_AsAdmin_CancelsScheduledAppointments()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/patients/1");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var activePatients = await (await _client.GetAsync("/api/v1/patients")).Content
            .ReadFromJsonAsync<List<PatientResponseDto>>(TestWebAppFactory.JsonOptions);
        activePatients.Should().NotContain(p => p.Id == 1);

        var allAppointments = await (await _client.GetAsync("/api/v1/appointments")).Content
            .ReadFromJsonAsync<List<AppointmentResponseDto>>(TestWebAppFactory.JsonOptions);
        allAppointments.Should().Contain(a => a.Id == 1 && a.Status == Status.Cancelled);
    }

    [Fact]
    public async Task DeactivatePatient_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/patients/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReactivatePatient_AsAdmin_RestoresPatient()
    {
        await LoginAsAdminAsync();

        var response = await _client.PatchAsync("/api/v1/patients/4/reactivate", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/patients/4");
        verify.StatusCode.Should().Be(HttpStatusCode.OK);
        var patient = await verify.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);
        patient!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ReactivatePatient_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.PatchAsync("/api/v1/patients/9999/reactivate", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteMyProfile_AsAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.DeleteAsync("/api/v1/patients/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMyProfile_AsAdmin_ReturnsForbidden()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/patients/me");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteMyProfile_AsPatient_DeactivatesSelf()
    {
        var register = new RegisterPatientRequestDto
        {
            Email = "selfdelete@test.com",
            Password = "Patient123",
            FirstName = "Self",
            LastName = "Delete",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1990, 1, 1),
            Religion = Religion.None,
            SocialSecurityNumber = "77788899900",
            TaxNumber = "TX777",
            DriverLicenseNumber = "DL777",
            MedicalInsuranceNumber = "MI777"
        };
        var registerResp = await _client.PostAsJsonAsync("/api/v1/auth/register/patient", register);
        var created = await registerResp.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);

        await LoginAsAsync("selfdelete@test.com", "Patient123");

        var response = await _client.DeleteAsync("/api/v1/patients/me");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await LoginAsAdminAsync();
        var verify = await _client.GetAsync($"/api/v1/patients/all/{created!.Id}");
        verify.StatusCode.Should().Be(HttpStatusCode.OK);
        var patient = await verify.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);
        patient!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteMyProfile_AsDeactivatedPatientWithStaleToken_ReturnsForbidden()
    {
        var register = new RegisterPatientRequestDto
        {
            Email = "staledelete@test.com",
            Password = "Patient123",
            FirstName = "Stale",
            LastName = "Delete",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            Religion = Religion.None,
            SocialSecurityNumber = "12121212100",
            TaxNumber = "TX121",
            DriverLicenseNumber = "DL121",
            MedicalInsuranceNumber = "MI121"
        };
        var registerResp = await _client.PostAsJsonAsync("/api/v1/auth/register/patient", register);
        var created = await registerResp.Content.ReadFromJsonAsync<PatientResponseDto>(TestWebAppFactory.JsonOptions);

        await LoginAsAsync("staledelete@test.com", "Patient123");
        var staleAuth = _client.DefaultRequestHeaders.Authorization;

        await LoginAsAdminAsync();
        var deactivate = await _client.DeleteAsync($"/api/v1/patients/{created!.Id}");
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Authorization = staleAuth;
        var response = await _client.DeleteAsync("/api/v1/patients/me");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
