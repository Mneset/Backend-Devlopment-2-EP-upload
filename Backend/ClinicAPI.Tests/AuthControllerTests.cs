using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicAPI.Models.Dtos.Auth;
using ClinicAPI.Models.Dtos.Patient;
using ClinicAPI.Models.Entities;
using FluentAssertions;

namespace ClinicAPI.Tests;

public class AuthControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidAdminCredentials_ReturnsTokenAndOk()
    {
        var request = new LoginRequestDto { Email = "admin@clinic.com", Password = "Admin123" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        body.Should().NotBeNull();
        body.Token.Should().NotBeNullOrEmpty();
        body.Role.Should().Be(Role.Admin);
        body.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithValidPatientCredentials_ReturnsTokenAndOk()
    {
        var request = new LoginRequestDto { Email = "jonas@email.com", Password = "Patient123" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        body.Should().NotBeNull();
        body.Token.Should().NotBeNullOrEmpty();
        body.Role.Should().Be(Role.Patient);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto { Email = "admin@clinic.com", Password = "WrongPassword" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto { Email = "nobody@nowhere.com", Password = "Admin123" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithGuestEmail_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto { Email = "guest@email.com", Password = "anything" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithMissingFields_ReturnsBadRequest()
    {
        var request = new LoginRequestDto { Email = "", Password = "" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterPatient_WithValidData_ReturnsCreatedAndRegisteredPatient()
    {
        var request = new RegisterPatientRequestDto
        {
            Email = "newpatient@test.com",
            Password = "Password123",
            FirstName = "New",
            LastName = "Patient",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            Religion = Religion.PreferNotToSay,
            SocialSecurityNumber = "123456789",
            TaxNumber = "TX001",
            DriverLicenseNumber = "DL001",
            MedicalInsuranceNumber = "MI001"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register/patient", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<PatientResponseDto>();
        body.Should().NotBeNull();
        body.Id.Should().BeGreaterThan(0);
        body.FirstName.Should().Be("New");
        body.LastName.Should().Be("Patient");
        body.IsRegistered.Should().BeTrue();
        body.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterPatient_WithDuplicateEmail_ReturnsConflict()
    {
        var request = new RegisterPatientRequestDto
        {
            Email = "admin@clinic.com",
            Password = "Password123",
            FirstName = "Dup",
            LastName = "User",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1990, 1, 1),
            Religion = Religion.PreferNotToSay,
            SocialSecurityNumber = "999999999",
            TaxNumber = "TX999",
            DriverLicenseNumber = "DL999",
            MedicalInsuranceNumber = "MI999"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register/patient", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task RegisterPatient_NewlyRegisteredUserCanLogIn()
    {
        var registerRequest = new RegisterPatientRequestDto
        {
            Email = "loginafter@test.com",
            Password = "LoginAfter123",
            FirstName = "Login",
            LastName = "After",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1992, 7, 4),
            Religion = Religion.PreferNotToSay,
            SocialSecurityNumber = "555555555",
            TaxNumber = "TX555",
            DriverLicenseNumber = "DL555",
            MedicalInsuranceNumber = "MI555"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register/patient", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginRequest = new LoginRequestDto { Email = "loginafter@test.com", Password = "LoginAfter123" };
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await loginResponse.Content.ReadFromJsonAsync<TokenResponseDto>();
        token.Should().NotBeNull();
        token.Role.Should().Be(Role.Patient);
    }

    [Fact]
    public async Task RegisterGuest_WithValidData_ReturnsCreatedAndUnregisteredPatient()
    {
        var request = new CreateGuestPatientRequestDto
        {
            FirstName = "Guest",
            LastName = "Caller",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(2000, 3, 14)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register/guest", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<PatientResponseDto>();
        body.Should().NotBeNull();
        body.Id.Should().BeGreaterThan(0);
        body.FirstName.Should().Be("Guest");
        body.IsRegistered.Should().BeFalse();
        body.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterGuest_WithMissingFields_ReturnsBadRequest()
    {
        var request = new CreateGuestPatientRequestDto
        {
            FirstName = "",
            LastName = "",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register/guest", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_AsDeactivatedPatient_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto { Email = "inactive@test.com", Password = "Patient123" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
