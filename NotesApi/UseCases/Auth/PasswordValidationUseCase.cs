using System.Text.RegularExpressions;
using NotesApi.Shared.Auth;

namespace NotesApi.UseCases.Auth;

public partial class PasswordValidationUseCase
{
    // 1- More than 6 chars
    // 2- At least 1 Upper and 1 Number

    private readonly Regex _regexUppercase;
    private readonly Regex _regexLowercase;
    private readonly Regex _regexNumber;

    public PasswordValidationUseCase()
    {
        _regexNumber = RegexNumber();
        _regexLowercase = RegexLower();
        _regexUppercase = RegexUpper();
    }

    public bool PasswordValidationLength(string password)
    {
        return password.Length >= 6;
    }

    public bool PasswordValidationContent_Uppercase(string password)
    {
        var match = _regexUppercase.Match(password);

        return match.Success;
    }

    public bool PasswordValidationContent_Lowercase(string password)
    {
        var match = _regexLowercase.Match(password);

        return match.Success;
    }

    public bool PasswordValidationContent_Number(string password)
    {
        var match = _regexNumber.Match(password);

        return match.Success;
    }

    public List<string> PasswordValidation(string password)
    {
        var errors = new List<string>();

        if (!PasswordValidationLength(password))
            errors.Add(AuthErrorsEnum.InvalidLength);

        if (!PasswordValidationContent_Uppercase(password))
            errors.Add(AuthErrorsEnum.InvalidContentUpper);

        if (!PasswordValidationContent_Lowercase(password))
            errors.Add(AuthErrorsEnum.InvalidContentLower);

        if (!PasswordValidationContent_Number(password))
            errors.Add(AuthErrorsEnum.InvalidContentNumber);

        return errors;
    }

    [GeneratedRegex("^(?=.*\\d)(?!.*\\s).*$")]
    private static partial Regex RegexNumber();

    [GeneratedRegex("^(?=.*[a-z])(?!.*\\s).*$")]
    private static partial Regex RegexLower();

    [GeneratedRegex("^(?=.*[A-Z])(?!.*\\s).*$")]
    private static partial Regex RegexUpper();
}
