using System.ComponentModel.DataAnnotations;

namespace NotesApi.Shared.Models;

public class Note
{
    public int Id { get; set; }
    public string NoteTitle { get; set; }
    public string NoteBody { get; set; }
    public DateTime NoteCreated { get; set; }
    public DateTime NoteModified { get; set; }
    public int userId { get; set; }
    public User User { get; set; }
}