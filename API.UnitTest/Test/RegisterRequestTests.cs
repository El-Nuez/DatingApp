using System.ComponentModel.DataAnnotations;
using API.DTOs;

public class RegisterRequestTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void RegisterRequest_WithValidProperties_ShouldPassValidation()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "validuser",
            Password = "pass1234"
        };

        // Act
        var validationResults = ValidateModel(registerRequest);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void RegisterRequest_WithEmptyUsername_ShouldFailValidation()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = string.Empty, // Invalid username (Required attribute)
            Password = "pass1234"
        };

        // Act
        var validationResults = ValidateModel(registerRequest);

        // Assert
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Username") && v.ErrorMessage.Contains("required"));
    }

    [Fact]
    public void RegisterRequest_WithShortPassword_ShouldFailValidation()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "validuser",
            Password = "123" // Invalid password (Too short)
        };

        // Act
        var validationResults = ValidateModel(registerRequest);

        // Assert
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Password") && v.ErrorMessage.Contains("minimum length of 4"));
    }

    [Fact]
    public void RegisterRequest_WithLongPassword_ShouldFailValidation()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "validuser",
            Password = "longpassword" // Invalid password (Too long)
        };

        // Act
        var validationResults = ValidateModel(registerRequest);

        // Assert
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Password") && v.ErrorMessage.Contains("maximum length of 8"));
    }
}
