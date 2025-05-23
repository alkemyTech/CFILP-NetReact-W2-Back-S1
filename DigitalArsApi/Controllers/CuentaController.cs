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

namespace DigitalArsApi.Controllers
{
    [Route("Cuenta")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly DigitalArsContext _context;

        public CuentaController(DigitalArsContext context)
        {
            _context = context;
        }

        // GET: api/Cuenta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuentas()
        {
            if (_context.Cuentas == null)
            {
                return NotFound();
            }

            var cuentas = await _context.Cuentas
                .Include(c => c.Usuario)
                .ToListAsync();

            return Ok(cuentas);
        }

        // GET: api/Cuenta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cuenta>> GetCuenta(int id)
        {
            if (_context.Cuentas == null)
            {
                return NotFound();
            }
            var cuenta = await _context.Cuentas.FindAsync(id);

            if (cuenta == null)
            {
                return NotFound();
            }

            return cuenta;
        }

        // PUT: api/Cuenta/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutCuenta(int id, Cuenta cuenta)
        {
            if (id != cuenta.Numero)
            {
                return BadRequest();
            }

            _context.Entry(cuenta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentaExists(id))
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

        // POST: api/Cuenta
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<Cuenta>> PostCuenta(Cuenta cuenta)
        {
            if (_context.Cuentas == null)
            {
                return Problem("Entity set 'DigitalArsContext.Cuentas'  is null.");
            }
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCuenta", new { id = cuenta.Numero }, cuenta);
        }

        // DELETE: api/Cuenta/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteCuenta(int id)
        {
            if (_context.Cuentas == null)
            {
                return NotFound();
            }
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound();
            }

            _context.Cuentas.Remove(cuenta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CuentaExists(int id)
        {
            return (_context.Cuentas?.Any(e => e.Numero == id)).GetValueOrDefault();
        }
    }
}
