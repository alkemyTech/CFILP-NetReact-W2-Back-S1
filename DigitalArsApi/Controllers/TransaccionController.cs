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
    [Route("Transaccion")]
    [ApiController]
    public class TransaccionController : ControllerBase
    {
        private readonly DigitalArsContext _context;

        public TransaccionController(DigitalArsContext context)
        {
            _context = context;
        }

        // GET: api/Transaccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaccion>>> GetTransacciones()
        {
            if (_context.Transacciones == null)
            {
                return NotFound();
            }

            var transacciones = await _context.Transacciones
                .Include(t => t.CuentaOrigen!)
                    .ThenInclude(c => c.Usuario!)
                .Include(t => t.CuentaDestino!)
                    .ThenInclude(c => c.Usuario!)
                .Include(t => t.Tipo!)
                .Include(t => t.PlazoFijo!)
                .ToListAsync();

            return transacciones;
        }

        // GET: api/Transaccion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaccion>> GetTransaccion(int id)
        {
            if (_context.Transacciones == null)
            {
                return NotFound();
            }
            var transaccion = await _context.Transacciones.FindAsync(id);

            if (transaccion == null)
            {
                return NotFound();
            }

            return transaccion;
        }

        // PUT: api/Transaccion/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PutTransaccion(int id, Transaccion transaccion)
        {
            if (id != transaccion.Id)
            {
                return BadRequest();
            }

            _context.Entry(transaccion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransaccionExists(id))
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

        // POST: api/Transaccion
        [HttpPost]
        [Authorize(Roles = "Administrador, Usuario")]
        public async Task<ActionResult<Transaccion>> PostTransaccion(Transaccion transaccion)
        {
            if (_context.Transacciones == null)
            {
                return Problem("Entity set 'DigitalArsContext.Transacciones' is null.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validar que las cuentas existan
                var cuentaDestino = await _context.Cuentas.FindAsync(transaccion.CtaDestino);
                if (cuentaDestino == null)
                    return BadRequest("Cuenta destino no encontrada.");

                Cuenta? cuentaOrigen = null;
                if (transaccion.CtaOrigen.HasValue)
                {
                    cuentaOrigen = await _context.Cuentas.FindAsync(transaccion.CtaOrigen.Value);
                    if (cuentaOrigen == null)
                        return BadRequest("Cuenta origen no encontrada.");

                    // Validar que tenga saldo suficiente
                    if (cuentaOrigen.Saldo < transaccion.Monto)
                        return BadRequest("Saldo insuficiente en la cuenta origen.");

                    // Descontar saldo
                    cuentaOrigen.Saldo -= transaccion.Monto;
                    cuentaOrigen.F_Update = DateTime.Now;
                    _context.Cuentas.Update(cuentaOrigen);
                }

                // Sumar saldo a la cuenta destino
                cuentaDestino.Saldo += transaccion.Monto;
                cuentaDestino.F_Update = DateTime.Now;
                _context.Cuentas.Update(cuentaDestino);

                // Guardar transacción
                transaccion.Fecha = DateTime.Now;
                _context.Transacciones.Add(transaccion);

                // Persistir cambios
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction("GetTransaccion", new { id = transaccion.Id }, transaccion);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Logging opcional
                return StatusCode(500, $"Error al procesar la transacción: {ex.Message}");
            }
        }

        // DELETE: api/Transaccion/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteTransaccion(int id)
        {
            if (_context.Transacciones == null)
            {
                return NotFound();
            }
            var transaccion = await _context.Transacciones.FindAsync(id);
            if (transaccion == null)
            {
                return NotFound();
            }

            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransaccionExists(int id)
        {
            return (_context.Transacciones?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
