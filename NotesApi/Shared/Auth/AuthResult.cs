namespace NotesApi.Shared.Auth;

public class AuthResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public bool Result { get; set; }
    public List<string> Errors { get; set; }
    public object User { get; set; }
}