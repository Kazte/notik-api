using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApi.Shared.Models;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public ICollection<Role> Roles { get; set; }

    public ICollection<Note> Notes { get; set; }
}