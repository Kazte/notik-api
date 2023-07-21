using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Configurations;
using NotesApi.Data.Interfaces;
using NotesApi.Shared.Auth;
using NotesApi.Shared.DTO;
using NotesApi.Shared.Models;
using NotesApi.UseCases.Auth;
using NotesApi.Utilities;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly INotesAppContext _context;

    private readonly JwtConfig _jwtConfig;
    private readonly JwtUtilities _jwtUtilities;

    private readonly PasswordValidationUseCase _passwordValidationUseCase;

    public AuthenticationController(INotesAppContext context, IOptions<JwtConfig> jwtConfig, JwtUtilities jwtUtilities)
    {
        _context = context;
        _jwtConfig = jwtConfig.Value;
        _jwtUtilities = jwtUtilities;
        _passwordValidationUseCase = new PasswordValidationUseCase();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid Model State.");

        var passwordValidation = _passwordValidationUseCase.PasswordValidation(request.Password);

        if (passwordValidation.Count > 0)
            return BadRequest(new AuthResult { Errors = passwordValidation, Result = false });

        var users = await _context.GetUsers();
        var emailUsed = users.Any(x => x.Email == request.Email);
        var usernameUsed = users.Any(x => x.Username == request.Username);

        var errors = new List<string>();

        if (emailUsed)
        {
            return BadRequest(new AuthResult { Result = false, Errors = new List<string> { "Email already taken" } });
        }

        if (usernameUsed)
        {
            return BadRequest(new AuthResult
                { Result = false, Errors = new List<string> { "Username already taken" } });
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User();
        user.Username = request.Username;
        user.PasswordHash = passwordHash;
        user.Email = request.Email;

        user.Role = await _context.GetRoleByName("User");

        await _context.PostUser(user);

        return Ok(new { result = true });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid Model State.");

        var user = (await _context.GetUsers()).FirstOrDefault(u => u.Username == request.Username);

        if (user is null)
            return NotFound(
                new AuthResult
                {
                    Errors = new List<string> { "User not found." }, // TODO: "Invalid Credentials". Only for Dev
                    Result = false
                }
            );

        if (user.Username != request.Username)
            return BadRequest(
                new AuthResult
                {
                    Errors = new List<string> { "Invalid Credentials." },
                    Result = false
                }
            );

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return BadRequest(
                new AuthResult
                {
                    Errors = new List<string>
                        { "Invalid Credentials.pass" }, // TODO: "Invalid Credentials". Only for Dev
                    Result = false
                }
            );

        var token = _jwtUtilities.CreateJwtToken(user);

        // Response.Cookies.Append("jwt", token, new CookieOptions() { HttpOnly = true });

        return Ok(
            new AuthResult
            {
                Result = true,
                Token = token,
                User = new
                {
                    userId = user.Id,
                    username = user.Username,
                    email = user.Email
                }
            }
        );
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(VerifyDto request)
    {
        try
        {
            var tokenSecurityHandler = new JwtSecurityTokenHandler();

            var tokenToVerify = tokenSecurityHandler.ValidateToken(
                request.Token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_jwtConfig.Secret)
                    )
                },
                out var validatedToken
            );

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256Signature,
                    StringComparison.InvariantCultureIgnoreCase
                );

                if (!result || tokenToVerify is null)
                {
                    return BadRequest(new { response = false, message = "Invalid Token" });
                }

                var token = tokenSecurityHandler.ReadJwtToken(request.Token);
                var claims = token.Claims.ToList().FirstOrDefault(c => c.Type == "id")?.Value;

                // return Ok(new
                // {
                //     response = false, message = "Invalid Token"
                // });

                return Ok(new { response = true, user = new { id = claims } });
            }
        }
        catch (Exception)
        {
            return BadRequest(new { response = false, message = "Invalid Token.err" });
        }

        return BadRequest();
    }
}