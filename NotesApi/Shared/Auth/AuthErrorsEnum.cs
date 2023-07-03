namespace NotesApi.Shared.Auth;

public static class AuthErrorsEnum
{
    public const string InvalidLength = "Invalid Length";
    public const string InvalidContentUpper = "Invalid Content - Password must have at least one Uppercase";
    public const string InvalidContentLower = "Invalid Content - Password must have at least one Lowercase";
    public const string InvalidContentNumber = "Invalid Content - Password must have at least one Number";
}