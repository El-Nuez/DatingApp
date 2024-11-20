using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DataEntities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UserRepositoryTests
{
    private readonly DataContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new DataContext(options);
        _context.Database.EnsureDeleted(); // Eliminar la base de datos antes de crearla.
        _context.Database.EnsureCreated(); // Volver a crearla para cada prueba.

        _userRepository = new UserRepository(_context, null!); // Pasa `null` para el mapper si no lo usas.
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<AppUser>
        {
            new AppUser
            {
                Id = 1,
                UserName = "user1",
                KnownAs = "User 1",
                Gender = "Male",
                City = "City1",
                Country = "Country1",
                Created = DateTime.UtcNow,
                LastActive = DateTime.UtcNow,
                PasswordHash = new byte[] { 0x01 },
                PasswordSalt = new byte[] { 0x02 }
            },
            new AppUser
            {
                Id = 2,
                UserName = "user2",
                KnownAs = "User 2",
                Gender = "Female",
                City = "City2",
                Country = "Country2",
                Created = DateTime.UtcNow,
                LastActive = DateTime.UtcNow,
                PasswordHash = new byte[] { 0x03 },
                PasswordSalt = new byte[] { 0x04 }
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.UserName == "user1");
        Assert.Contains(result, u => u.UserName == "user2");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectUser()
    {
        // Arrange
        var user = new AppUser
        {
            Id = 1,
            UserName = "testuser",
            KnownAs = "Test User",
            Gender = "Male",
            City = "Test City",
            Country = "Test Country",
            Created = DateTime.UtcNow,
            LastActive = DateTime.UtcNow,
            PasswordHash = new byte[] { 0x01 },
            PasswordSalt = new byte[] { 0x02 }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.UserName);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnCorrectUser()
    {
        // Arrange
        var user = new AppUser
        {
            Id = 1,
            UserName = "testuser",
            KnownAs = "Test User",
            Gender = "Male",
            City = "Test City",
            Country = "Test Country",
            Created = DateTime.UtcNow,
            LastActive = DateTime.UtcNow,
            PasswordHash = new byte[] { 0x01 },
            PasswordSalt = new byte[] { 0x02 }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.UserName);
    }

    [Fact]
    public async Task SaveAllAsync_ShouldReturnTrue_WhenChangesAreSaved()
    {
        // Arrange
        var user = new AppUser
        {
            Id = 1,
            UserName = "user1",
            KnownAs = "User 1",
            Gender = "Male",
            City = "City1",
            Country = "Country1",
            Created = DateTime.UtcNow,
            LastActive = DateTime.UtcNow,
            PasswordHash = new byte[] { 0x01 },
            PasswordSalt = new byte[] { 0x02 }
        };

        _context.Users.Add(user);

        // Act
        var result = await _userRepository.SaveAllAsync();

        // Assert
        Assert.True(result);
    }
}
