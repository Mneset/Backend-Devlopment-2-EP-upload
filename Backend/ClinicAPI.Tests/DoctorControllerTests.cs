using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicAPI.Models.Dtos.Appointment;
using ClinicAPI.Models.Dtos.Auth;
using ClinicAPI.Models.Dtos.Doctor;
using ClinicAPI.Models.Entities;
using FluentAssertions;

namespace ClinicAPI.Tests;

public class DoctorControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public DoctorControllerTests(TestWebAppFactory factory)
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
        var body = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    [Fact]
    public async Task GetAllDoctors_AsAnonymous_ReturnsActiveSeededDoctors()
    {
        var response = await _client.GetAsync("/api/v1/doctors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var doctors = await response.Content.ReadFromJsonAsync<List<DoctorResponseDto>>();
        doctors.Should().HaveCount(4);
        doctors.Should().OnlyContain(d => d.IsActive);
    }

    [Fact]
    public async Task GetDoctorById_WithValidActiveId_ReturnsDoctor()
    {
        var response = await _client.GetAsync("/api/v1/doctors/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var doctor = await response.Content.ReadFromJsonAsync<DoctorResponseDto>();
        doctor!.Id.Should().Be(1);
        doctor.FirstName.Should().Be("Erik");
        doctor.LastName.Should().Be("Hansen");
        doctor.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetDoctorById_WithInactiveId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/doctors/5");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDoctorById_WithNonExistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/doctors/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchDoctors_WithPartialName_ReturnsMatching()
    {
        var response = await _client.GetAsync("/api/v1/doctors/search?name=Han");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var doctors = await response.Content.ReadFromJsonAsync<List<DoctorResponseDto>>();
        doctors.Should().Contain(d => d.LastName == "Hansen");
    }

    [Fact]
    public async Task SearchDoctors_OnlyReturnsActiveDoctors()
    {
        var response = await _client.GetAsync("/api/v1/doctors/search?name=Sol");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var doctors = await response.Content.ReadFromJsonAsync<List<DoctorResponseDto>>();
        doctors.Should().NotContain(d => d.Id == 5);
    }

    [Fact]
    public async Task SearchDoctors_WithEmptyName_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v1/doctors/search?name=");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllDoctorsIncludingInactive_AsAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/doctors/all");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllDoctorsIncludingInactive_AsPatient_ReturnsForbidden()
    {
        await LoginAsPatientAsync();

        var response = await _client.GetAsync("/api/v1/doctors/all");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllDoctorsIncludingInactive_AsAdmin_ReturnsAllDoctorsIncludingInactive()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/doctors/all");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var doctors = await response.Content.ReadFromJsonAsync<List<DoctorResponseDto>>();
        doctors.Should().HaveCount(5);
        doctors.Should().Contain(d => d.Id == 5 && !d.IsActive);
    }

    [Fact]
    public async Task GetDoctorByIdIncludingInactive_AsAdmin_ReturnsInactiveDoctor()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/doctors/all/5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var doctor = await response.Content.ReadFromJsonAsync<DoctorResponseDto>();
        doctor!.Id.Should().Be(5);
        doctor.FirstName.Should().Be("Magnus");
        doctor.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task CreateDoctor_AsAdmin_ReturnsCreated()
    {
        await LoginAsAdminAsync();

        var request = new CreateDoctorRequestDto
        {
            FirstName = "Test",
            LastName = "Doctor",
            Gender = Gender.Male,
            SpecialityId = 1,
            ClinicId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/doctors", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<DoctorResponseDto>();
        created!.FirstName.Should().Be("Test");
        created.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateDoctor_AsAnonymous_ReturnsUnauthorized()
    {
        var request = new CreateDoctorRequestDto
        {
            FirstName = "Test",
            LastName = "Doctor",
            Gender = Gender.Male,
            SpecialityId = 1,
            ClinicId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/doctors", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDoctor_WithInvalidSpeciality_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new CreateDoctorRequestDto
        {
            FirstName = "Test",
            LastName = "Doctor",
            Gender = Gender.Male,
            SpecialityId = 9999,
            ClinicId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/doctors", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDoctor_WithInvalidClinic_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new CreateDoctorRequestDto
        {
            FirstName = "Test",
            LastName = "Doctor",
            Gender = Gender.Male,
            SpecialityId = 1,
            ClinicId = 9999
        };

        var response = await _client.PostAsJsonAsync("/api/v1/doctors", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDoctor_AsAdmin_UpdatesFields()
    {
        await LoginAsAdminAsync();

        var request = new UpdateDoctorRequestDto
        {
            Id = 1,
            FirstName = "Updated",
            LastName = "Hansen",
            Gender = Gender.Male,
            SpecialityId = 1,
            ClinicId = 1
        };

        var response = await _client.PutAsJsonAsync("/api/v1/doctors/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/doctors/1");
        var updated = await verify.Content.ReadFromJsonAsync<DoctorResponseDto>();
        updated!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeactivateDoctor_AsAdmin_CancelsScheduledAppointments()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/doctors/1");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var activeDoctors = await (await _client.GetAsync("/api/v1/doctors")).Content
            .ReadFromJsonAsync<List<DoctorResponseDto>>();
        activeDoctors.Should().NotContain(d => d.Id == 1);

        var allAppointments = await (await _client.GetAsync("/api/v1/appointments")).Content
            .ReadFromJsonAsync<List<AppointmentResponseDto>>();
        allAppointments.Should().Contain(a => a.Id == 1 && a.Status == Status.Cancelled);
    }

    [Fact]
    public async Task ReactivateDoctor_AsAdmin_RestoresDoctor()
    {
        await LoginAsAdminAsync();

        var response = await _client.PatchAsync("/api/v1/doctors/5/reactivate", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/doctors/5");
        verify.StatusCode.Should().Be(HttpStatusCode.OK);
        var doctor = await verify.Content.ReadFromJsonAsync<DoctorResponseDto>();
        doctor!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetDoctorByIdIncludingInactive_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/doctors/all/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDoctor_WithIdMismatch_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new UpdateDoctorRequestDto
        {
            Id = 2,
            FirstName = "Mismatched",
            LastName = "Hansen",
            Gender = Gender.Male,
            SpecialityId = 1,
            ClinicId = 1
        };

        var response = await _client.PutAsJsonAsync("/api/v1/doctors/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDoctor_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var request = new UpdateDoctorRequestDto
        {
            Id = 9999,
            FirstName = "Doesnt",
            LastName = "Exist",
            Gender = Gender.Male,
            SpecialityId = 1,
            ClinicId = 1
        };

        var response = await _client.PutAsJsonAsync("/api/v1/doctors/9999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDoctor_WithInvalidSpeciality_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new UpdateDoctorRequestDto
        {
            Id = 1,
            FirstName = "Erik",
            LastName = "Hansen",
            Gender = Gender.Male,
            SpecialityId = 9999,
            ClinicId = 1
        };

        var response = await _client.PutAsJsonAsync("/api/v1/doctors/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDoctor_WithInvalidClinic_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new UpdateDoctorRequestDto
        {
            Id = 1,
            FirstName = "Erik",
            LastName = "Hansen",
            Gender = Gender.Male,
            SpecialityId = 1,
            ClinicId = 9999
        };

        var response = await _client.PutAsJsonAsync("/api/v1/doctors/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeactivateDoctor_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/doctors/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReactivateDoctor_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.PatchAsync("/api/v1/doctors/9999/reactivate", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
