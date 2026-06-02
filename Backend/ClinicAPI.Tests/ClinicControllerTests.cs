namespace ClinicAPI.Tests;

public class ClinicControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    
     public ClinicControllerTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task X()
    {
       
    }
}
