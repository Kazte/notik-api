namespace NotesApi.Shared.DTO.Notes;

public class NotePutDto
{
    public int Id { get; set; }
    public string NoteTitle { get; set; }
    public string NoteBody { get; set; }
    public DateTime NoteCreated { get; set; }
    public DateTime NoteModified { get; set; }
    public int userId { get; set; }
}