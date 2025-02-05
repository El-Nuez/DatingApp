using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using API;
using API.DTOs;
using API.UnitTests;
using Xunit;

public class UsersControllerTests : IClassFixture<APIWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public UsersControllerTests(APIWebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateClient();
    }

    // Método para autenticar al cliente HTTP y establecer el token
    private async Task AuthenticateAsync()
    {
        var loginRequest = new LoginRequest
        {
            Username = "arenita", // Usa un usuario válido de tu base de datos
            Password = "123456"   // La contraseña correspondiente
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/account/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var loginData = await loginResponse.Content.ReadAsStringAsync();
        var userResponse = JsonSerializer.Deserialize<UserResponse>(loginData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userResponse.Token);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnListOfMembers()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();
        var members = JsonSerializer.Deserialize<List<MemberResponse>>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(members);
    }

    [Fact]
    public async Task GetByUsernameAsync_UserFound_ShouldReturnMember()
    {
        // Arrange
        await AuthenticateAsync();
        var username = "arenita"; // Usa un usuario válido en tu base de datos

        // Act
        var response = await _client.GetAsync($"/api/users/{username}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();
        var member = JsonSerializer.Deserialize<MemberResponse>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(member);
        Assert.Equal(username, member.UserName);
    }

    [Fact]
    public async Task GetByUsernameAsync_UserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        await AuthenticateAsync();
        var username = "NonExistentUser";

        // Act
        var response = await _client.GetAsync($"/api/users/{username}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        await AuthenticateAsync();

        var updateRequest = new MemberUpdateRequest
        {
            Introduction = "Nueva introducción",
            LookingFor = "Algo nuevo",
            Interests = "Programación, Música",
            City = "Ciudad X",
            Country = "País Y"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        await AuthenticateAsync();

        // Envía un objeto nulo para provocar un error de BadRequest
        MemberUpdateRequest updateRequest = null;

        // Act
        var response = await _client.PutAsJsonAsync("/api/users", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
