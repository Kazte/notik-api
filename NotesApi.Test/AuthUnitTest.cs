using NotesApi.Shared.Auth;
using NotesApi.UseCases.Auth;

namespace NotesApi.Test;

public class AuthUnitTest
{
    private PasswordValidationUseCase _passwordValidation;

    public AuthUnitTest()
    {
        _passwordValidation = new PasswordValidationUseCase();
    }


    [Theory]
    [InlineData("abcde", false)]
    [InlineData("abcdef", true)]
    public void Validate_Password_Length(string password, bool expected)
    {
        // Arrange
        var validator = _passwordValidation;


        // Act
        var result = validator.PasswordValidationLength(password);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Abc", true)]
    [InlineData("abc", false)]
    public void Validate_Password_Content_Uppercase(string password, bool expected)
    {
        // Arrange
        var validator = _passwordValidation;

        // Act
        var result = validator.PasswordValidationContent_Uppercase(password);

        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("abc", true)]
    [InlineData("ABC", false)]
    public void Validate_Password_Content_Lowercase(string password, bool expected)
    {
        // Arrange
        var validator = _passwordValidation;

        // Act
        var result = validator.PasswordValidationContent_Lowercase(password);

        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("aBc1", true)]
    [InlineData("aBc", false)]
    public void Validate_Password_Content_Number(string password, bool expected)
    {
        // Arrange
        var validator = _passwordValidation;

        // Act
        var result = validator.PasswordValidationContent_Number(password);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Abcdef123", new string[] { })]
    [InlineData("abcdef123", new string[] { AuthErrorsEnum.InvalidContentUpper})]
    [InlineData("ABCDEFG123", new string[] {AuthErrorsEnum.InvalidContentLower })]
    [InlineData("Abcdef", new string[] {AuthErrorsEnum.InvalidContentNumber })]
    public void Validate_Password_Content_Whole(string password, string[] errors)
    {
        // Arrange
        var validator = _passwordValidation;

        // Act
        var result = validator.PasswordValidation(password);

        // Assert
        Assert.Equal(errors, result);
    }
}