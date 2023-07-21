using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApi.Shared.Models;

[Table("Notes")]
public class Note
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Guid { get; set; }
    public string NoteTitle { get; set; } = "";
    public string NoteBody { get; set; } = "";
    public DateTime NoteCreated { get; set; } = DateTime.UtcNow;
    public DateTime NoteModified { get; set; } = DateTime.UtcNow;
    public bool Public { get; set; }

    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }
}
