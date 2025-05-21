using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalArsApi.Data;
using DigitalArsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DigitalArsApi.Controllers
{
    [Route("Usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DigitalArsContext _context;
        // private readonly IPasswordHasher<Usuario> _passwordHasher;
// , IPasswordHasher<Usuario> passwordHasher
        public UsuarioController(DigitalArsContext context)
        {
            _context = context;
            // _passwordHasher = passwordHasher;
        }

        // GET: api/Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }

            var usuariosConRoles = await _context.Usuarios
                .Include(u => u.Roles) // ⬅️ clave para que cargue la relación
                .Include(u => u.Cuentas)
                .ToListAsync();

            return Ok(usuariosConRoles);
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Roles)
                .Include(u => u.Cuentas) // opcional, si también querés incluir las cuentas
                .FirstOrDefaultAsync(u => u.DNI == id); // reemplaza a FindAsync

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuario/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.DNI)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuario
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Usuario>> PostUsuario([FromBody] Usuario usuarioData)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'DigitalArsContext.Usuarios' is null.");
            }

            var hasher = new PasswordHasher<Usuario>();
            string hashedPassword = hasher.HashPassword(usuarioData, usuarioData.Password);

            var newUser = new Usuario
            {
                DNI = usuarioData.DNI,
                Nombre = usuarioData.Nombre,
                Apellido = usuarioData.Apellido,
                Email = usuarioData.Email,
                Password = hashedPassword,
                Fecha = DateTime.Now,
                F_Update = DateTime.Now
            };

            if (usuarioData.Roles != null && usuarioData.Roles.Any())
            {
                foreach (var rolRecibido in usuarioData.Roles)
                {
                    var rolExistente = await _context.Roles.FindAsync(rolRecibido.Id);

                    if (rolExistente != null)
                    {
                        newUser.Roles.Add(rolExistente);
                    }
                    else
                    {
                        ModelState.AddModelError("Roles", $"Rol with Id {rolRecibido.Id} does not exist.");
                        return BadRequest(ModelState);
                    }
                }
            }

            _context.Usuarios.Add(newUser);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = newUser.DNI }, newUser);
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.DNI == id)).GetValueOrDefault();
        }
    }
}
