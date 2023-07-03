using System.Runtime.InteropServices.JavaScript;
using System.Text.RegularExpressions;
using NotesApi.Shared.Auth;

namespace NotesApi.UseCases.Auth;

public class PasswordValidationUseCase
{
    // 1- More than 6 chars
    // 2- At least 1 Upper and 1 Number

    private const int MINIMUM_LENGTH = 6;

    private Regex _regexUppercase;
    private Regex _regexLowercase;
    private Regex _regexNumber;
    
    // ^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$
    
    public PasswordValidationUseCase()
    {
        _regexNumber = new Regex(@"^(?=.*\d)(?!.*\s).*$");
        _regexLowercase = new Regex(@"^(?=.*[a-z])(?!.*\s).*$");
        _regexUppercase = new Regex(@"^(?=.*[A-Z])(?!.*\s).*$");
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
}