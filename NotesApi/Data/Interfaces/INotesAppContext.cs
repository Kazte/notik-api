using NotesApi.Shared.Models;

namespace NotesApi.Data.Interfaces;

public interface INotesAppContext
{
    // Notes
    Task<IEnumerable<Note>> GetNotes();
    Task<Note?> GetNoteById(int noteId);
    Task<IEnumerable<Note>> GetNotesByUserId(int userId);
    Task<Note> PostNote(Note note);
    Task PutNote(Note note);
    Task<bool> DeleteNote(Note note);

    // Users
    Task<IEnumerable<User>> GetUsers();
    Task<User?> GetUserById(int userId);
    Task PostUser(User user);
    Task PutUser(User user);
    Task<bool> DeleteUser(User user);
    
    
    // Role
    Task<Role?> GetRoleById(int id);
}