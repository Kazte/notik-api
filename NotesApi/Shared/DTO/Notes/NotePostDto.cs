namespace NotesApi.Shared.DTO.Notes;

public class NotePostDto
{
    public int Id { get; set; }
    public string? Guid { get; set; }
    public string NoteTitle { get; set; }
    public string NoteBody { get; set; }
    public DateTime NoteCreated { get; set; }
    public DateTime NoteModified { get; set; } = DateTime.UtcNow;
    public bool Public { get; set; }
}