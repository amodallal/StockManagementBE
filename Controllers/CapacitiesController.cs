using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Data;
using StockManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapacitiesController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public CapacitiesController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/capacities
        [HttpGet]
        public IActionResult GetCapacities()
        {
            var items = _context.Capacities.ToList();
            return Ok(items);
        }

        // POST: api/capacities
        [HttpPost]
        public async Task<IActionResult> PostCapacity([FromBody] Capacity capacity)
        {
            if (capacity == null)
            {
                return BadRequest("Capacity is null");
            }

            // Check if required fields are missing
            if (string.IsNullOrEmpty(capacity.CapacityName))
            {
                return BadRequest("Capacity cannot be null or empty");
            }

            _context.Capacities.Add(capacity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostCapacity), new { id = capacity.CapacityID }, capacity);
        }

        // DELETE: api/capacities/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCapacity(int id)
        {
            var capacity = await _context.Capacities.FindAsync(id);
            if (capacity == null)
            {
                return NotFound("Capacity not found");
            }

            _context.Capacities.Remove(capacity);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content status
        }
    }
}
