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
    [Route("EstadoPlazoFijo")]
    [ApiController]
    public class EstadoPlazoFijoController : ControllerBase
    {
        private readonly DigitalArsContext _context;

        public EstadoPlazoFijoController(DigitalArsContext context)
        {
            _context = context;
        }

        // GET: api/EstadoPlazoFijo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstadoPlazoFijo>>> GetEstadosPlazoFijo()
        {
          if (_context.EstadosPlazoFijo == null)
          {
              return NotFound();
          }
            return await _context.EstadosPlazoFijo.ToListAsync();
        }

        // GET: api/EstadoPlazoFijo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EstadoPlazoFijo>> GetEstadoPlazoFijo(int id)
        {
          if (_context.EstadosPlazoFijo == null)
          {
              return NotFound();
          }
            var estadoPlazoFijo = await _context.EstadosPlazoFijo.FindAsync(id);

            if (estadoPlazoFijo == null)
            {
                return NotFound();
            }

            return estadoPlazoFijo;
        }

        // PUT: api/EstadoPlazoFijo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstadoPlazoFijo(int id, EstadoPlazoFijo estadoPlazoFijo)
        {
            if (id != estadoPlazoFijo.Id)
            {
                return BadRequest();
            }

            _context.Entry(estadoPlazoFijo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstadoPlazoFijoExists(id))
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

        // POST: api/EstadoPlazoFijo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EstadoPlazoFijo>> PostEstadoPlazoFijo(EstadoPlazoFijo estadoPlazoFijo)
        {
          if (_context.EstadosPlazoFijo == null)
          {
              return Problem("Entity set 'DigitalArsContext.EstadosPlazoFijo'  is null.");
          }
            _context.EstadosPlazoFijo.Add(estadoPlazoFijo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEstadoPlazoFijo", new { id = estadoPlazoFijo.Id }, estadoPlazoFijo);
        }

        // DELETE: api/EstadoPlazoFijo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstadoPlazoFijo(int id)
        {
            if (_context.EstadosPlazoFijo == null)
            {
                return NotFound();
            }
            var estadoPlazoFijo = await _context.EstadosPlazoFijo.FindAsync(id);
            if (estadoPlazoFijo == null)
            {
                return NotFound();
            }

            _context.EstadosPlazoFijo.Remove(estadoPlazoFijo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EstadoPlazoFijoExists(int id)
        {
            return (_context.EstadosPlazoFijo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
