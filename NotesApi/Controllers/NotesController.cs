using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Configurations;
using NotesApi.Data.Interfaces;
using NotesApi.Shared.DTO.Notes;
using NotesApi.Shared.Models;
using NotesApi.Utilities;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INotesAppContext _context;

    private readonly JwtUtilities _jwtUtilities;

    public NotesController(INotesAppContext context, JwtUtilities jwtUtilities)
    {
        _context = context;
        _jwtUtilities = jwtUtilities;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Get()
    {
        return Ok(await _context.GetNotes());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = HttpContext.User;

        int? userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        var note = await _context.GetNoteById(id);

        if (userId != note.UserId)
        {
            if (note.Public)
            {
                return Ok(note);
            }

            return BadRequest();
        }


        if (note is null)
            return NotFound();

        return Ok(note);
    }

    [HttpGet("GetByGuid")]
    public async Task<IActionResult> GetByGuid(string guid)
    {
        var user = HttpContext.User;

        int? userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
        var note = await _context.GetNoteByGuid(guid);

        if (userId != note.UserId)
        {
            if (note.Public)
            {
                return Ok(note);
            }

            return BadRequest();
        }


        if (note is null)
            return NotFound();

        return Ok(note);
    }

    [HttpGet("GetNotesByUserID")]
    public async Task<IActionResult> GetByUserId()
    {
        var user = HttpContext.User;

        int? id = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

        if (id == null)
            return NotFound();

        var notes = await _context.GetNotesByUserId(id.Value);

        return Ok(notes);
    }


    [HttpPost]
    public async Task<IActionResult> Post(NotePostDto request)
    {
        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var user = await _context.GetUserById(userId);

        if (user is null)
            return NotFound();

        var newNote = new Note
        {
            UserId = user.Id,
            NoteBody = request.NoteBody,
            NoteTitle = request.NoteTitle,
            Guid = GuidUtilities.GenerateGuid(),
            NoteModified = request.NoteModified,
            Public = request.Public
        };

        var n = await _context.PostNote(newNote);
        return CreatedAtAction("Post", n.Id, new
        {
            Id = n.Id,
            UserId = user.Id,
            request.NoteBody,
            request.NoteTitle,
            request.NoteCreated,
            request.NoteModified,
            request.Public,
            newNote.Guid,
        });
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] NotePostDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Error bad request.");

        var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var n = await _context.GetNoteById(request.Id);

        if (n != null)
        {
            if (userId != n.UserId)
                return BadRequest();

            n.NoteTitle = request.NoteTitle;
            n.NoteBody = request.NoteBody;
            n.Public = request.Public;
            n.NoteModified = request.NoteModified;

            await _context.SaveChanges();
        }
        else
        {
            return NotFound();
        }

        return Ok(new { result = true });
    }

    [HttpDelete("{noteId:int}")]
    public async Task<IActionResult> Delete(int noteId)
    {
        var user = HttpContext.User;
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

        var note = await _context.GetNoteById(noteId);

        if (note.UserId != userId)
        {
            return BadRequest();
        }


        await _context.DeleteNote(note);
        return Ok(new { result = true });
    }

    [HttpGet("share")]
    [AllowAnonymous]
    public async Task<IActionResult> Share(string guid)
    {
        var user = HttpContext.User;
        var identifier = user.FindFirst(ClaimTypes.NameIdentifier);

        var note = await _context.GetNoteByGuid(guid);

        if (note is null)
            return NotFound();
        
        if (note.Public)
        {
            Console.WriteLine("Test");
            return Ok(new { data = note, result = true });
        }
        
        if (identifier is null)
        {
            if (note.Public)
                return Ok(note);

            return NotFound();
        }

        int? userId = int.Parse(identifier.Value);

        if (userId == note.UserId)
            return Ok(new { data = note, result = true });

        

        return BadRequest();
    }
}