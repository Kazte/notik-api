using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApi.Shared.Models;

[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public int RoleId { get;set; }
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; }
           
    public virtual ICollection<Note> Notes { get; set; }
}