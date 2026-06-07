using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicAPI.Models.Dtos.AppointmentCategory;
using ClinicAPI.Models.Dtos.Auth;
using FluentAssertions;

namespace ClinicAPI.Tests;

public class AppointmentCategoryControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public AppointmentCategoryControllerTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task LoginAsAdminAsync()
    {
        await LoginAsAsync("admin@clinic.com", "Admin123");
    }

    private async Task LoginAsAsync(string email, string password)
    {
        var loginRequest = new LoginRequestDto { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var body = await response.Content.ReadFromJsonAsync<TokenResponseDto>(TestWebAppFactory.JsonOptions);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    [Fact]
    public async Task GetAllAppointmentCategories_AsAnonymous_ReturnsSeededCategories()
    {
        var response = await _client.GetAsync("/api/v1/appointmentcategories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<AppointmentCategoryResponseDto>>(TestWebAppFactory.JsonOptions);
        categories.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetAppointmentCategoryById_AsAnonymous_WithValidId_ReturnsCategory()
    {
        var response = await _client.GetAsync("/api/v1/appointmentcategories/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var category = await response.Content.ReadFromJsonAsync<AppointmentCategoryResponseDto>(TestWebAppFactory.JsonOptions);
        category!.Id.Should().Be(1);
        category.Name.Should().Be("General Checkup");
    }

    [Fact]
    public async Task GetAppointmentCategoryById_WithNonExistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/appointmentcategories/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAppointmentCategory_AsAdmin_ReturnsCreated()
    {
        await LoginAsAdminAsync();

        var request = new CreateAppointmentCategoryRequestDto { Name = "Test Category" };

        var response = await _client.PostAsJsonAsync("/api/v1/appointmentcategories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<AppointmentCategoryResponseDto>(TestWebAppFactory.JsonOptions);
        created!.Name.Should().Be("Test Category");
        created.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateAppointmentCategory_AsAnonymous_ReturnsUnauthorized()
    {
        var request = new CreateAppointmentCategoryRequestDto { Name = "Should not be created" };

        var response = await _client.PostAsJsonAsync("/api/v1/appointmentcategories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateAppointmentCategory_AsAdmin_UpdatesName()
    {
        await LoginAsAdminAsync();

        var request = new UpdateAppointmentCategoryRequestDto { Id = 1, Name = "Updated General Checkup" };

        var response = await _client.PutAsJsonAsync("/api/v1/appointmentcategories/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/appointmentcategories/1");
        var updated = await verify.Content.ReadFromJsonAsync<AppointmentCategoryResponseDto>(TestWebAppFactory.JsonOptions);
        updated!.Name.Should().Be("Updated General Checkup");
    }

    [Fact]
    public async Task UpdateAppointmentCategory_WithIdMismatch_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new UpdateAppointmentCategoryRequestDto { Id = 2, Name = "Mismatched" };

        var response = await _client.PutAsJsonAsync("/api/v1/appointmentcategories/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAppointmentCategory_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var request = new UpdateAppointmentCategoryRequestDto { Id = 9999, Name = "Does not exist" };

        var response = await _client.PutAsJsonAsync("/api/v1/appointmentcategories/9999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAppointmentCategory_WithDependentAppointments_ReturnsConflict()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/appointmentcategories/1");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteAppointmentCategory_WithoutDependencies_ReturnsNoContent()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/appointmentcategories/4");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/appointmentcategories/4");
        verify.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAppointmentCategory_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/appointmentcategories/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
