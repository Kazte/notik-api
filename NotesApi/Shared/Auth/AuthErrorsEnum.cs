namespace NotesApi.Shared.Auth;

public static class AuthErrorsEnum
{
    public const string InvalidLength = "Password must have at least 6 Chars";
    public const string InvalidContentUpper = "Password must have at least one Uppercase";
    public const string InvalidContentLower = "Password must have at least one Lowercase";
    public const string InvalidContentNumber = "Password must have at least one Number";
}