using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicAPI.Models.Dtos.Auth;
using ClinicAPI.Models.Dtos.Speciality;
using FluentAssertions;

namespace ClinicAPI.Tests;

public class SpecialityControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public SpecialityControllerTests(TestWebAppFactory factory)
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
        var body = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    [Fact]
    public async Task GetAllSpecialities_AsAnonymous_ReturnsSeededSpecialities()
    {
        var response = await _client.GetAsync("/api/v1/specialities");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var specialities = await response.Content.ReadFromJsonAsync<List<SpecialityResponseDto>>();
        specialities.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetSpecialityById_AsAnonymous_WithValidId_ReturnsSpeciality()
    {
        var response = await _client.GetAsync("/api/v1/specialities/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var speciality = await response.Content.ReadFromJsonAsync<SpecialityResponseDto>();
        speciality!.Id.Should().Be(1);
        speciality.Name.Should().Be("Cardiology");
    }

    [Fact]
    public async Task GetSpecialityById_WithNonExistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/specialities/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSpeciality_AsAdmin_ReturnsCreated()
    {
        await LoginAsAdminAsync();

        var request = new CreateSpecialityRequestDto { Name = "Test Speciality" };

        var response = await _client.PostAsJsonAsync("/api/v1/specialities", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<SpecialityResponseDto>();
        created!.Name.Should().Be("Test Speciality");
        created.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateSpeciality_AsAnonymous_ReturnsUnauthorized()
    {
        var request = new CreateSpecialityRequestDto { Name = "Should not be created" };

        var response = await _client.PostAsJsonAsync("/api/v1/specialities", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateSpeciality_AsAdmin_UpdatesName()
    {
        await LoginAsAdminAsync();

        var request = new UpdateSpecialityRequestDto { Id = 1, Name = "Updated Cardiology" };

        var response = await _client.PutAsJsonAsync("/api/v1/specialities/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/specialities/1");
        var updated = await verify.Content.ReadFromJsonAsync<SpecialityResponseDto>();
        updated!.Name.Should().Be("Updated Cardiology");
    }

    [Fact]
    public async Task UpdateSpeciality_WithIdMismatch_ReturnsBadRequest()
    {
        await LoginAsAdminAsync();

        var request = new UpdateSpecialityRequestDto { Id = 2, Name = "Mismatched" };

        var response = await _client.PutAsJsonAsync("/api/v1/specialities/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSpeciality_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var request = new UpdateSpecialityRequestDto { Id = 9999, Name = "Does not exist" };

        var response = await _client.PutAsJsonAsync("/api/v1/specialities/9999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSpeciality_WithDependentDoctors_ReturnsConflict()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/specialities/1");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteSpeciality_WithoutDependencies_ReturnsNoContent()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/specialities/5");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verify = await _client.GetAsync("/api/v1/specialities/5");
        verify.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSpeciality_WithNonExistentId_ReturnsNotFound()
    {
        await LoginAsAdminAsync();

        var response = await _client.DeleteAsync("/api/v1/specialities/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
