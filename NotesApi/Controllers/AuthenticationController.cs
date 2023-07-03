using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Configurations;
using NotesApi.Data.Interfaces;
using NotesApi.Shared.Auth;
using NotesApi.Shared.DTO;
using NotesApi.Shared.Models;
using NotesApi.UseCases.Auth;
using NuGet.Common;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    public readonly INotesAppContext _context;

    private readonly JwtConfig _jwtConfig;

    private readonly PasswordValidationUseCase _passwordValidationUseCase;

    public AuthenticationController(INotesAppContext context, IOptions<JwtConfig> jwtConfig)
    {
        _context = context;
        _jwtConfig = jwtConfig.Value;
        _passwordValidationUseCase = new PasswordValidationUseCase();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid Model State.");

        var passwordValidation = _passwordValidationUseCase.PasswordValidation(request.Password);

        if (passwordValidation.Count > 0)
            return BadRequest(new AuthResult
            {
                Errors = passwordValidation,
                Result = false
            });


        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User();
        user.Username = request.Username;
        user.PasswordHash = passwordHash;
        user.Email = request.Email;

        await _context.PostUser(user);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid Model State.");

        var user = (await _context.GetUsers()).FirstOrDefault(u => u.Username == request.Username);

        if (user.Username != request.Username)
            return BadRequest(new AuthResult
            {
                Errors = new List<string> { "Invalid Credentials." },
                Result = false
            });

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return BadRequest(new AuthResult
            {
                Errors = new List<string> { "Invalid Credentials." }, // TODO: "Invalid Credentials". Only for Dev
                Result = false
            });

        var token = CreateJwtToken(user);

        Response.Cookies.Append("jwt", token, new CookieOptions()
        {
            HttpOnly = true
        });

        return Ok(new AuthResult
        {
            Result = true,
            Token = token
        });
    }

    private string CreateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new("id", user.Id.ToString())
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtConfig.Secret));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}