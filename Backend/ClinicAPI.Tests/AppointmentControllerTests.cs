using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicAPI.Models.Dtos.Appointment;
using ClinicAPI.Models.Dtos.Auth;
using ClinicAPI.Models.Entities;
using FluentAssertions;

namespace ClinicAPI.Tests;

public class AppointmentControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public AppointmentControllerTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task LoginAsAdminAsync()
    {
        await LoginAsAsync("admin@clinic.com", "Admin123");
    }

    private async Task LoginAsJonasAsync()
    {
        await LoginAsAsync("jonas@email.com", "Patient123");
    }

    private async Task LoginAsMarieAsync()
    {
        await LoginAsAsync("marie@email.com", "Patient123");
    }

    private async Task LoginAsAsync(string email, string password)
    {
        var loginRequest = new LoginRequestDto { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var body = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    [Fact]
    public async Task GetAllAppointments_AsAnonymous_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/appointments");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllAppointments_AsPatient_ReturnsForbidden()
    {
        await LoginAsJonasAsync();

        var response = await _client.GetAsync("/api/v1/appointments");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllAppointments_AsAdmin_ReturnsAllSeededAppointments()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/appointments");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentResponseDto>>();
        appointments.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAppointments_AsAdminWithStatusFilter_ReturnsFilteredAppointments()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/appointments?status=Scheduled");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentResponseDto>>();
        appointments.Should().OnlyContain(a => a.Status == Status.Scheduled);
        appointments.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMyAppointments_AsJonas_ReturnsOnlyJonasAppointments()
    {
        await LoginAsJonasAsync();

        var response = await _client.GetAsync("/api/v1/appointments/mine");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentResponseDto>>();
        appointments.Should().NotBeEmpty();
        appointments.Should().OnlyContain(a => a.PatientId == 1);
    }

    [Fact]
    public async Task GetAppointmentById_AsOwner_ReturnsAppointment()
    {
        await LoginAsJonasAsync();

        var response = await _client.GetAsync("/api/v1/appointments/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var appointment = await response.Content.ReadFromJsonAsync<AppointmentResponseDto>();
        appointment!.PatientId.Should().Be(1);
    }

    [Fact]
    public async Task GetAppointmentById_AsOtherPatient_ReturnsForbidden()
    {
        await LoginAsMarieAsync();

        var response = await _client.GetAsync("/api/v1/appointments/1");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAppointmentById_AsAdmin_ReturnsAnyAppointment()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/appointments/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BookAppointment_AsPatient_ReturnsCreated()
    {
        await LoginAsJonasAsync();

        var request = new BookAppointmentRequestDto
        {
            DoctorId = 2,
            ClinicId = 2,
            AppointmentCategoryId = 1,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PostAsJsonAsync("/api/v1/appointments", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var appointment = await response.Content.ReadFromJsonAsync<AppointmentResponseDto>();
        appointment!.PatientId.Should().Be(1);
        appointment.DoctorId.Should().Be(2);
        appointment.Status.Should().Be(Status.Scheduled);
    }

    [Fact]
    public async Task BookAppointment_WithDoctorClinicMismatch_ReturnsBadRequest()
    {
        await LoginAsJonasAsync();

        var request = new BookAppointmentRequestDto
        {
            DoctorId = 1,
            ClinicId = 2,
            AppointmentCategoryId = 1,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PostAsJsonAsync("/api/v1/appointments", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BookAppointment_WithOverlappingTime_ReturnsConflict()
    {
        await LoginAsJonasAsync();

        var request = new BookAppointmentRequestDto
        {
            DoctorId = 1,
            ClinicId = 1,
            AppointmentCategoryId = 1,
            AppointmentDate = new DateTime(2026, 6, 10, 9, 15, 0),
            DurationInMin = 30
        };

        var response = await _client.PostAsJsonAsync("/api/v1/appointments", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CancelAppointment_AsOwner_ReturnsNoContentAndCancelsStatus()
    {
        await LoginAsJonasAsync();

        var response = await _client.PatchAsync("/api/v1/appointments/1/cancel", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/appointments/1");
        var appointment = await verify.Content.ReadFromJsonAsync<AppointmentResponseDto>();
        appointment!.Status.Should().Be(Status.Cancelled);
    }

    [Fact]
    public async Task CancelAppointment_AsOtherPatient_ReturnsForbidden()
    {

        await LoginAsMarieAsync();

        var response = await _client.PatchAsync("/api/v1/appointments/1/cancel", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CompleteAppointment_AsAdmin_ReturnsNoContent()
    {
        await LoginAsAdminAsync();

        var response = await _client.PatchAsync("/api/v1/appointments/1/complete", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/appointments/1");
        var appointment = await verify.Content.ReadFromJsonAsync<AppointmentResponseDto>();
        appointment!.Status.Should().Be(Status.Completed);
    }

    [Fact]
    public async Task CompleteAppointment_AsPatient_ReturnsForbidden()
    {
        await LoginAsJonasAsync();

        var response = await _client.PatchAsync("/api/v1/appointments/1/complete", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RescheduleAppointment_AsOwner_ReturnsNoContentAndUpdatesTime()
    {
        await LoginAsJonasAsync();

        var newDate = new DateTime(2026, 12, 15, 10, 0, 0);
        var request = new RescheduleAppointmentRequestDto
        {
            Id = 1,
            AppointmentDate = newDate,
            DurationInMin = 45
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/appointments/1/reschedule", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/appointments/1");
        var appointment = await verify.Content.ReadFromJsonAsync<AppointmentResponseDto>();
        appointment!.AppointmentDate.Should().Be(newDate);
        appointment.DurationInMin.Should().Be(45);
    }

    [Fact]
    public async Task RescheduleAppointment_WithIdMismatch_ReturnsBadRequest()
    {
        await LoginAsJonasAsync();

        var request = new RescheduleAppointmentRequestDto
        {
            Id = 99,
            AppointmentDate = new DateTime(2026, 12, 15, 10, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/appointments/1/reschedule", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RescheduleAppointment_AsOtherPatient_ReturnsForbidden()
    {
        await LoginAsMarieAsync();

        var request = new RescheduleAppointmentRequestDto
        {
            Id = 1,
            AppointmentDate = new DateTime(2026, 12, 15, 10, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/appointments/1/reschedule", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RescheduleAppointment_WithOverlappingTime_ReturnsConflict()
    {
        await LoginAsJonasAsync();

        var bookRequest = new BookAppointmentRequestDto
        {
            DoctorId = 2,
            ClinicId = 2,
            AppointmentCategoryId = 1,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 0, 0),
            DurationInMin = 30
        };
        var bookResponse = await _client.PostAsJsonAsync("/api/v1/appointments", bookRequest);
        bookResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var reschedule = new RescheduleAppointmentRequestDto
        {
            Id = 1,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 15, 0),
            DurationInMin = 30
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/appointments/1/reschedule", reschedule);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetAppointmentById_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.GetAsync("/api/v1/appointments/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CancelAppointment_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.PatchAsync("/api/v1/appointments/9999/cancel", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CompleteAppointment_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.PatchAsync("/api/v1/appointments/9999/complete", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RescheduleAppointment_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var request = new RescheduleAppointmentRequestDto
        {
            Id = 9999,
            AppointmentDate = new DateTime(2026, 12, 15, 10, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/appointments/9999/reschedule", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RescheduleAppointment_OnCancelledAppointment_ReturnsConflict()
    {
        await LoginAsJonasAsync();

        var cancel = await _client.PatchAsync("/api/v1/appointments/1/cancel", content: null);
        cancel.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var request = new RescheduleAppointmentRequestDto
        {
            Id = 1,
            AppointmentDate = new DateTime(2026, 12, 15, 10, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/appointments/1/reschedule", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task BookAppointment_WithNonExistentDoctor_ReturnsBadRequest()
    {
        await LoginAsJonasAsync();

        var request = new BookAppointmentRequestDto
        {
            DoctorId = 9999,
            ClinicId = 1,
            AppointmentCategoryId = 1,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PostAsJsonAsync("/api/v1/appointments", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BookAppointment_WithNonExistentCategory_ReturnsBadRequest()
    {
        await LoginAsJonasAsync();

        var request = new BookAppointmentRequestDto
        {
            DoctorId = 1,
            ClinicId = 1,
            AppointmentCategoryId = 9999,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PostAsJsonAsync("/api/v1/appointments", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BookAppointment_AsAdmin_ReturnsForbidden()
    {

        await LoginAsAdminAsync();

        var request = new BookAppointmentRequestDto
        {
            DoctorId = 1,
            ClinicId = 1,
            AppointmentCategoryId = 1,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PostAsJsonAsync("/api/v1/appointments", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task BookAppointment_WithInactivePatient_ReturnsBadRequest()
    {
        await LoginAsJonasAsync();
        var jonasAuth = _client.DefaultRequestHeaders.Authorization;

        await LoginAsAdminAsync();
        var deactivate = await _client.DeleteAsync("/api/v1/patients/1");
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Authorization = jonasAuth;

        var request = new BookAppointmentRequestDto
        {
            DoctorId = 2,
            ClinicId = 2,
            AppointmentCategoryId = 1,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PostAsJsonAsync("/api/v1/appointments", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task BookAppointment_WithInactiveClinic_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();
        var deactivateClinic = await _client.DeleteAsync("/api/v1/clinics/1");
        deactivateClinic.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var reactivateDoctor = await _client.PatchAsync("/api/v1/doctors/1/reactivate", content: null);
        reactivateDoctor.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await LoginAsJonasAsync();

        var request = new BookAppointmentRequestDto
        {
            DoctorId = 1,
            ClinicId = 1,
            AppointmentCategoryId = 1,
            AppointmentDate = new DateTime(2026, 12, 1, 14, 0, 0),
            DurationInMin = 30
        };

        var response = await _client.PostAsJsonAsync("/api/v1/appointments", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CancelAppointment_AsDeactivatedPatientWithStaleToken_ReturnsForbidden()
    {
        await LoginAsJonasAsync();
        var jonasAuth = _client.DefaultRequestHeaders.Authorization;

        await LoginAsAdminAsync();
        var deactivate = await _client.DeleteAsync("/api/v1/patients/1");
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Authorization = jonasAuth;
        var response = await _client.PatchAsync("/api/v1/appointments/1/cancel", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RescheduleAppointment_AsDeactivatedPatientWithStaleToken_ReturnsForbidden()
    {
        await LoginAsJonasAsync();
        var jonasAuth = _client.DefaultRequestHeaders.Authorization;

        await LoginAsAdminAsync();
        var deactivate = await _client.DeleteAsync("/api/v1/patients/1");
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        _client.DefaultRequestHeaders.Authorization = jonasAuth;
        var request = new RescheduleAppointmentRequestDto
        {
            Id = 1,
            AppointmentDate = new DateTime(2026, 12, 15, 10, 0, 0),
            DurationInMin = 30
        };
        var response = await _client.PatchAsJsonAsync("/api/v1/appointments/1/reschedule", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
