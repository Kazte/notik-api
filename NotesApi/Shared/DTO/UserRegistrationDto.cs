using System.ComponentModel.DataAnnotations;

namespace NotesApi.Shared.DTO;

public class UserRegistrationDto
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Email { get; set; }
}