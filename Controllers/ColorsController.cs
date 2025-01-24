using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;
using System.Threading.Tasks;
using System.Linq;
using StockManagement.Data;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public ColorsController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/colors
        [HttpGet]
        public async Task<IActionResult> GetColors()
        {
            var colors = await _context.Colors.ToListAsync();
            return Ok(colors);
        }

        // GET: api/colors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);

            if (color == null)
            {
                return NotFound();
            }

            return Ok(color);
        }

        // POST: api/colors
        [HttpPost]
        public async Task<IActionResult> PostColor([FromBody] Color color)
        {
            if (color == null)
            {
                return BadRequest("Color data is null.");
            }

            // Optional: Validate if the color already exists
            if (_context.Colors.Any(c => c.ColorName.ToLower() == color.ColorName.ToLower()))
            {
                return Conflict("A color with this name already exists.");
            }

            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetColor", new { id = color.ColorId }, color);
        }

        // PUT: api/colors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutColor(int id, [FromBody] Color color)
        {
            if (id != color.ColorId)
            {
                return BadRequest();
            }

            _context.Entry(color).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Colors.Any(e => e.ColorId == id))
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

        // DELETE: api/colors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
