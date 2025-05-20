using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using DigitalArsApi.Models;
using DigitalArsApi.Data;

namespace DigitalArsApi.Controllers;

[ApiController]
[Route("Token")]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly DigitalArsContext _context;

    public TokenController(IConfiguration config, DigitalArsContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> GenerateToken([FromQuery] string email, [FromQuery] string password)
    {
        // Validar existencia del usuario
        var usuario = await _context.Usuarios
            .Include(u => u.Roles)
            .Include(u => u.Cuentas)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
            return Unauthorized("Usuario no encontrado");

        // Validar contraseña
        var hasher = new PasswordHasher<Usuario>();
        var resultado = hasher.VerifyHashedPassword(usuario, usuario.Password, password);

        if (resultado == PasswordVerificationResult.Failed)
            return Unauthorized("Contraseña incorrecta");

        // Construcción de claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        foreach (var rol in usuario.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol.Nombre));
        }

        // Validar configuración JWT
        var jwtKey = _config["Jwt:Key"];
        var jwtIssuer = _config["Jwt:Issuer"];
        var jwtAudience = _config["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
            return StatusCode(500, "Configuración JWT incompleta en appsettings.json");

        // Generar token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Devolver token + datos del usuario
        return Ok(new
        {
            token = tokenString,
            usuarioDNI = usuario.DNI
        });
    }
}
