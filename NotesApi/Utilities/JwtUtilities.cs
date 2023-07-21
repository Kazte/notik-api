using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Configurations;
using NotesApi.Shared.Models;

namespace NotesApi.Utilities;

public class JwtUtilities
{
    private readonly JwtConfig _jwtConfig;

    public JwtUtilities(IOptions<JwtConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
    }

    public string CreateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

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

    public List<Claim> GetClaimsFromJwt(string? jwt)
    {
        var tokenSecurityHandler = new JwtSecurityTokenHandler();

        if (_jwtConfig.Secret != null)
        {
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
        }
        else
        {
            throw new NullReferenceException("JwtConfig secret is Null");
        }

        var token = tokenSecurityHandler.ReadJwtToken(jwt);
        var claims = token.Claims.ToList();
        return claims;
    }

    public static string GetJwtFromRequest(HttpRequest request)
    {
        var jwt = request.Headers["Authorization"];
        return jwt.ToString().Split(' ')[1];
    }
}