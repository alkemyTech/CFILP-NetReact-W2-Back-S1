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

        public async Task<ActionResult<Transaccion>> PostTransaccion([FromBody] Transaccion transaccion)
        {
            if (_context.Transacciones == null || _context.Cuentas == null || _context.PlazosFijos == null || _context.EstadosPlazoFijo == null)
            {
                return Problem("Uno o más conjuntos de entidades (Transacciones, Cuentas, PlazosFijos, EstadosPlazoFijo) son nulos en el contexto.");
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                transaccion.Fecha = DateTime.Now;
                _context.Transacciones.Add(transaccion);

                if (transaccion.IdTipo == 3) // Tipo: Transferencia
                {
                    if (!transaccion.CtaOrigen.HasValue)
                    {
                        return BadRequest("Para transferencias, la cuenta origen es obligatoria.");
                    }

                    var cuentaOrigen = await _context.Cuentas.FindAsync(transaccion.CtaOrigen.Value);
                    var cuentaDestino = await _context.Cuentas.FindAsync(transaccion.CtaDestino);

                    if (cuentaOrigen == null)
                    {
                        return BadRequest("Cuenta origen no encontrada.");
                    }
                    if (cuentaDestino == null)
                    {
                        return BadRequest("Cuenta destino no encontrada.");
                    }
                    if (cuentaOrigen.Numero == cuentaDestino.Numero)
                    {
                        return BadRequest("No se puede transferir a la misma cuenta.");
                    }

                    if (cuentaOrigen.Saldo < transaccion.Monto)
                    {
                        return BadRequest("Saldo insuficiente en la cuenta origen para la transferencia.");
                    }

                    cuentaOrigen.Saldo -= transaccion.Monto;
                    cuentaOrigen.F_Update = DateTime.Now;
                    _context.Cuentas.Update(cuentaOrigen);

                    cuentaDestino.Saldo += transaccion.Monto;
                    cuentaDestino.F_Update = DateTime.Now;
                    _context.Cuentas.Update(cuentaDestino);

                    if (string.IsNullOrWhiteSpace(transaccion.Descripcion))
                    {
                        transaccion.Descripcion = "Transferencia de fondos";
                    }
                }
                else if (transaccion.IdTipo == 1) // Tipo: Depósito
                {
                    if (transaccion.CtaDestino <= 0)
                    {
                        return BadRequest("Para depósitos, la cuenta destino es obligatoria y debe ser un número de cuenta válido.");
                    }
                    if (transaccion.Monto <= 0)
                    {
                        return BadRequest("El monto del depósito debe ser mayor a cero.");
                    }

                    var cuentaDestino = await _context.Cuentas.FindAsync(transaccion.CtaDestino);
                    
                    if (cuentaDestino == null)
                    {
                        return BadRequest("Cuenta destino para el depósito no encontrada.");
                    }

                    cuentaDestino.Saldo += transaccion.Monto;
                    cuentaDestino.F_Update = DateTime.Now;
                    _context.Cuentas.Update(cuentaDestino);

                    if (string.IsNullOrWhiteSpace(transaccion.Descripcion))
                    {
                        transaccion.Descripcion = $"Depósito de fondos desde cuenta {transaccion.CtaOrigen?.ToString() ?? "Desconocida"}";
                    }
                }
                else if (transaccion.IdTipo == 2) // Tipo: Inversión a Plazo Fijo
                {
                    if (!transaccion.CtaOrigen.HasValue || transaccion.CtaOrigen.Value != transaccion.CtaDestino)
                    {
                        return BadRequest("Para inversiones, la cuenta origen y destino deben ser la misma.");
                    }

                    var cuentaInversion = await _context.Cuentas.FindAsync(transaccion.CtaOrigen.Value);

                    if (cuentaInversion == null)
                    {
                        return BadRequest("Cuenta de inversión no encontrada.");
                    }

                    if (cuentaInversion.Saldo < transaccion.Monto)
                    {
                        return BadRequest("Saldo insuficiente en la cuenta para realizar la inversión.");
                    }

                    int diasPlazoFijo = 0;
                    if (!string.IsNullOrEmpty(transaccion.Descripcion) && transaccion.Descripcion.Contains("días"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(transaccion.Descripcion, @"por (\d+)\s*días");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int parsedDays))
                        {
                            diasPlazoFijo = parsedDays;
                        }
                    }

                    if (diasPlazoFijo <= 0)
                    {
                        return BadRequest("No se pudo determinar el plazo en días para la inversión. Asegúrese de que la descripción contenga 'por X días'.");
                    }

                    cuentaInversion.Saldo -= transaccion.Monto;
                    cuentaInversion.F_Update = DateTime.Now;
                    _context.Cuentas.Update(cuentaInversion);

                    await _context.SaveChangesAsync();

                    const decimal TASA_ANUAL_DEFAULT = 0.32m;
                    DateTime fechaVencimiento = DateTime.Now.AddDays(diasPlazoFijo);

                    decimal tasaDiaria = TASA_ANUAL_DEFAULT / 365m;
                    decimal interesEsperado = transaccion.Monto * tasaDiaria * diasPlazoFijo;

                    var estadoActivo = await _context.EstadosPlazoFijo.FirstOrDefaultAsync(e => e.Nombre == "Activo");
                    if (estadoActivo == null)
                    {
                        return StatusCode(500, "Estado 'Activo' para Plazo Fijo no encontrado en la base de datos. Asegúrese de que exista.");
                    }

                    var plazoFijo = new PlazoFijo
                    {
                        IdTransaccion = transaccion.Id, // Usamos el ID generado
                        Monto = transaccion.Monto,
                        TNA = TASA_ANUAL_DEFAULT,
                        F_Inicio = DateTime.Now,
                        F_Fin = fechaVencimiento,
                        InteresEsperado = interesEsperado,
                        IdEstado = estadoActivo.Id
                    };
                    _context.PlazosFijos.Add(plazoFijo);
                }
                else
                {
                    return BadRequest("Tipo de transacción no válido.");
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return CreatedAtAction("GetTransaccion", new { id = transaccion.Id }, transaccion);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                Console.Error.WriteLine($"Error al procesar la transacción: {ex.Message} - StackTrace: {ex.StackTrace}");
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
