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
    [Route("PlazoFijo")]
    [ApiController]
    public class PlazoFijoController : ControllerBase
    {
        private readonly DigitalArsContext _context;

        public PlazoFijoController(DigitalArsContext context)
        {
            _context = context;
        }

        // GET: api/PlazoFijo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlazoFijo>>> GetPlazosFijos()
        {
          if (_context.PlazosFijos == null)
          {
              return NotFound();
          }
            return await _context.PlazosFijos.ToListAsync();
        }

        // GET: api/PlazoFijo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlazoFijo>> GetPlazoFijo(int id)
        {
          if (_context.PlazosFijos == null)
          {
              return NotFound();
          }
            var plazoFijo = await _context.PlazosFijos.FindAsync(id);

            if (plazoFijo == null)
            {
                return NotFound();
            }

            return plazoFijo;
        }

        // PUT: api/PlazoFijo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlazoFijo(int id, PlazoFijo plazoFijo)
        {
            if (id != plazoFijo.Id)
            {
                return BadRequest();
            }

            _context.Entry(plazoFijo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlazoFijoExists(id))
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

        // POST: api/PlazoFijo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlazoFijo>> PostPlazoFijo(PlazoFijo plazoFijo)
        {
          if (_context.PlazosFijos == null)
          {
              return Problem("Entity set 'DigitalArsContext.PlazosFijos'  is null.");
          }
            _context.PlazosFijos.Add(plazoFijo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlazoFijo", new { id = plazoFijo.Id }, plazoFijo);
        }

        // DELETE: api/PlazoFijo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlazoFijo(int id)
        {
            if (_context.PlazosFijos == null)
            {
                return NotFound();
            }
            var plazoFijo = await _context.PlazosFijos.FindAsync(id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            _context.PlazosFijos.Remove(plazoFijo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlazoFijoExists(int id)
        {
            return (_context.PlazosFijos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
