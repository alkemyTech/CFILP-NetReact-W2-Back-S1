using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("Token")]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _config;

    public TokenController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")] // Cambio de "token" a "generate" para mejor claridad
    public IActionResult GenerateToken(string username, string password)
    {
        if (username == "admin" && password == "123456")
        {
            // Valida usuario y contraseña (¡solo ejemplo, NO para producción!)
            var claims = new[]
            {
                // Define datos del usuario ("claims")
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Administrador")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            // Obtiene clave secreta de la configuración

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // Crea credenciales de firma

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );
            // Crea el token JWT

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            // Devuelve el token serializado
        }

        return Unauthorized();
    }
}