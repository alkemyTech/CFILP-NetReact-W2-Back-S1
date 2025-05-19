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
        var usuario = await _context.Usuarios
            .Include(u => u.Roles)
            .Include(u => u.Cuentas)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
            return Unauthorized("Usuario no encontrado");

        var hasher = new PasswordHasher<Usuario>();
        var resultado = hasher.VerifyHashedPassword(usuario, usuario.Password, password);

        if (resultado == PasswordVerificationResult.Failed)
            return Unauthorized("Contraseña incorrecta");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        // Agregar roles como claims
        foreach (var rol in usuario.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol.Nombre));
        }

        foreach (var rol in usuario.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol.Nombre));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            user = new
            {
                usuario.DNI,
                usuario.Nombre,
                usuario.Apellido,
                usuario.Email,
                roles = usuario.Roles.Select(r => r.Nombre).ToList(),
                cuentas = usuario.Cuentas.Select(c => new
                {
                    c.Numero,
                    c.Saldo,
                    c.Fecha,
                    c.F_Update
                }).ToList()
            }
        });
    }
}