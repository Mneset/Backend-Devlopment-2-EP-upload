using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicAPI.Models.Dtos.Auth;
using ClinicAPI.Models.Dtos.Clinic;
using ClinicAPI.Models.Dtos.Doctor;
using ClinicAPI.Models.Dtos.Appointment;
using ClinicAPI.Models.Entities;
using FluentAssertions;

namespace ClinicAPI.Tests;

public class ClinicControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public ClinicControllerTests(TestWebAppFactory factory)
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
    public async Task GetAllClinics_AsAnonymous_ReturnsActiveSeededClinics()
    {
        var response = await _client.GetAsync("/api/v1/clinics");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clinics = await response.Content.ReadFromJsonAsync<List<ClinicResponseDto>>(TestWebAppFactory.JsonOptions);
        clinics.Should().NotBeNull();
        clinics.Should().HaveCount(5);
        clinics.Should().OnlyContain(c => c.IsActive);
    }

    [Fact]
    public async Task GetClinicById_WithValidId_ReturnsClinic()
    {
        var response = await _client.GetAsync("/api/v1/clinics/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clinic = await response.Content.ReadFromJsonAsync<ClinicResponseDto>(TestWebAppFactory.JsonOptions);
        clinic.Should().NotBeNull();
        clinic.Id.Should().Be(1);
        clinic.Name.Should().Be("Oslo University Hospital");
        clinic.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetClinicById_WithNonExistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/clinics/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllClinicsIncludingInactive_AsAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/clinics/all");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllClinicsIncludingInactive_AsPatient_ReturnsForbidden()
    {
        await LoginAsPatientAsync();

        var response = await _client.GetAsync("/api/v1/clinics/all");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllClinicsIncludingInactive_AsAdmin_ReturnsAllClinics()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/clinics/all");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clinics = await response.Content.ReadFromJsonAsync<List<ClinicResponseDto>>(TestWebAppFactory.JsonOptions);
        clinics.Should().HaveCount(5);
    }

    [Fact]
    public async Task CreateClinic_AsAdmin_ReturnsCreated()
    {
        await LoginAsAdminAsync();

        var request = new CreateClinicRequestDto { Name = "New Test Clinic" };

        var response = await _client.PostAsJsonAsync("/api/v1/clinics", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ClinicResponseDto>(TestWebAppFactory.JsonOptions);
        created.Should().NotBeNull();
        created.Id.Should().BeGreaterThan(0);
        created.Name.Should().Be("New Test Clinic");
        created.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateClinic_AsAdmin_UpdatesName()
    {
        await LoginAsAdminAsync();

        var request = new UpdateClinicRequestDto { Id = 1, Name = "Updated Name" };

        var response = await _client.PutAsJsonAsync("/api/v1/clinics/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/clinics/1");
        var updated = await verify.Content.ReadFromJsonAsync<ClinicResponseDto>(TestWebAppFactory.JsonOptions);
        updated!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateClinic_WithIdMismatch_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new UpdateClinicRequestDto { Id = 2, Name = "Mismatched" };

        var response = await _client.PutAsJsonAsync("/api/v1/clinics/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetClinicByIdIncludingInactive_AsAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/clinics/all/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetClinicByIdIncludingInactive_AsAdmin_ReturnsInactiveClinic()
    {
        await LoginAsAdminAsync();

        var deactivate = await _client.DeleteAsync("/api/v1/clinics/1");
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var response = await _client.GetAsync("/api/v1/clinics/all/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clinic = await response.Content.ReadFromJsonAsync<ClinicResponseDto>(TestWebAppFactory.JsonOptions);
        clinic.Should().NotBeNull();
        clinic.Id.Should().Be(1);
        clinic.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task CreateClinic_AsAnonymous_ReturnsUnauthorized()
    {
        var request = new CreateClinicRequestDto { Name = "Should not be created" };

        var response = await _client.PostAsJsonAsync("/api/v1/clinics", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ReactivateClinic_AsAdmin_RestoresClinic()
    {
        await LoginAsAdminAsync();

        var deactivate = await _client.DeleteAsync("/api/v1/clinics/1");
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var response = await _client.PatchAsync("/api/v1/clinics/1/reactivate", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/clinics/1");
        verify.StatusCode.Should().Be(HttpStatusCode.OK);
        var clinic = await verify.Content.ReadFromJsonAsync<ClinicResponseDto>(TestWebAppFactory.JsonOptions);
        clinic!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateClinic_AsAdmin_CascadesToDoctorsAndScheduledAppointments()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/clinics/1");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var activeClinics = await (await _client.GetAsync("/api/v1/clinics")).Content
            .ReadFromJsonAsync<List<ClinicResponseDto>>(TestWebAppFactory.JsonOptions);
        activeClinics.Should().NotContain(c => c.Id == 1);

        var activeDoctors = await (await _client.GetAsync("/api/v1/doctors")).Content
            .ReadFromJsonAsync<List<DoctorResponseDto>>(TestWebAppFactory.JsonOptions);
        activeDoctors.Should().NotContain(d => d.Id == 1);

        var allAppointments = await (await _client.GetAsync("/api/v1/appointments")).Content
            .ReadFromJsonAsync<List<AppointmentResponseDto>>(TestWebAppFactory.JsonOptions);
        allAppointments.Should().Contain(a => a.Id == 1 && a.Status == Status.Cancelled);
    }
}
