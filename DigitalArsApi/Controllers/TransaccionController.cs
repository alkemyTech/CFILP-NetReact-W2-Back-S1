using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalArsApi.Data;
using DigitalArsApi.Models;

namespace DigitalArsApi.Controllers
{
    [Route("api/[controller]")]
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
            return await _context.Transacciones.ToListAsync();
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transaccion>> PostTransaccion(Transaccion transaccion)
        {
          if (_context.Transacciones == null)
          {
              return Problem("Entity set 'DigitalArsContext.Transacciones'  is null.");
          }
            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaccion", new { id = transaccion.Id }, transaccion);
        }

        // DELETE: api/Transaccion/5
        [HttpDelete("{id}")]
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
