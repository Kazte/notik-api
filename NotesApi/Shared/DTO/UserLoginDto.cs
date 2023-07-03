using System.ComponentModel.DataAnnotations;

namespace NotesApi.Shared.DTO;

public class UserLoginDto
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}