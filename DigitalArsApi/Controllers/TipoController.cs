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
    public class TipoController : ControllerBase
    {
        private readonly DigitalArsContext _context;

        public TipoController(DigitalArsContext context)
        {
            _context = context;
        }

        // GET: api/Tipo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tipo>>> GetTipos()
        {
          if (_context.Tipos == null)
          {
              return NotFound();
          }
            return await _context.Tipos.ToListAsync();
        }

        // GET: api/Tipo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tipo>> GetTipo(int id)
        {
          if (_context.Tipos == null)
          {
              return NotFound();
          }
            var tipo = await _context.Tipos.FindAsync(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return tipo;
        }

        // PUT: api/Tipo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipo(int id, Tipo tipo)
        {
            if (id != tipo.Id)
            {
                return BadRequest();
            }

            _context.Entry(tipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoExists(id))
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

        // POST: api/Tipo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tipo>> PostTipo(Tipo tipo)
        {
          if (_context.Tipos == null)
          {
              return Problem("Entity set 'DigitalArsContext.Tipos'  is null.");
          }
            _context.Tipos.Add(tipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipo", new { id = tipo.Id }, tipo);
        }

        // DELETE: api/Tipo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipo(int id)
        {
            if (_context.Tipos == null)
            {
                return NotFound();
            }
            var tipo = await _context.Tipos.FindAsync(id);
            if (tipo == null)
            {
                return NotFound();
            }

            _context.Tipos.Remove(tipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoExists(int id)
        {
            return (_context.Tipos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
