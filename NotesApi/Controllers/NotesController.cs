using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Configurations;
using NotesApi.Data.Contexts;
using NotesApi.Data.Interfaces;
using NotesApi.Shared.DTO.Notes;
using NotesApi.Shared.Models;
using NuGet.Protocol;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INotesAppContext _context;

    private readonly JwtConfig _jwtConfig;

    public NotesController(INotesAppContext context, IOptions<JwtConfig> jwtConfig)
    {
        _context = context;
        _jwtConfig = jwtConfig.Value;
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
        var note = await _context.GetNoteById(id);

        if (note is null)
            return NotFound();

        return Ok(note);
    }

    [HttpGet("GetNotesByUserID/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var jwt = Request.Cookies["jwt"];

        if (jwt is null)
            return BadRequest("Invalid Token.");

        var claims = GetClaimsFromJwt(jwt);

        var idFromJwt = claims.FirstOrDefault(c => c.Type == "id")?.Value;

        var isAdmin =
            claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault(c => c.Value == "Admin")?.Value is not null;

        if (isAdmin)
        {
            var n = await _context.GetNotesByUserId(userId);

            return Ok(n);
        }


        if (idFromJwt is null)
            return BadRequest("Invalid Token.");

        if (userId != int.Parse(idFromJwt))
            return NotFound();

        var note = await _context.GetNotesByUserId(userId);

        return Ok(note);
    }

    private List<Claim> GetClaimsFromJwt(string? jwt)
    {
        var tokenSecurityHandler = new JwtSecurityTokenHandler();

        var tokenToVerify = tokenSecurityHandler.ValidateToken(jwt, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtConfig.Secret))
        }, out var validatedToken);

        if (validatedToken is JwtSecurityToken jwtSecurityToken)
        {
            var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature,
                StringComparison.InvariantCultureIgnoreCase);

            if (!result || tokenToVerify is null)
            {
                throw new Exception("Invalid Token");
            }
        }

        var token = tokenSecurityHandler.ReadJwtToken(jwt);
        var claims = token.Claims.ToList();
        return claims;
    }

    [HttpPost]
    public async Task<IActionResult> Post(NotePostDto request)
    {
        var user = await _context.GetUserById(request.userId);

        if (user is null)
            return NotFound();

        var newNote = new Note
        {
            userId = user.Id,
            NoteBody = request.NoteBody,
            NoteTitle = request.NoteTitle,
            NoteCreated = request.NoteCreated,
            NoteModified = request.NoteModified
        };

        await _context.PostNote(newNote);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Note request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Error bad request.");

        await _context.PutNote(request);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Note request)
    {
        await _context.DeleteNote(request);
        return NoContent();
    }
}