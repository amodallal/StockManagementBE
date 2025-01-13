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

        // GET: api/brand
        [HttpGet]
        public IActionResult GetTestData()
        {
            var items = _context.Capacities.ToList();
            return Ok(items);
        }


        [HttpPost]
        public async Task<IActionResult> PostCapacity([FromBody] Capacity Capacity)
        {
            if (Capacity == null)
            {
                return BadRequest("Capacity is null");
            }

            // Check if required fields are missing
            if (string.IsNullOrEmpty(Capacity.CapacityName))
            {
                return BadRequest("Capacity cannot be null or empty");
            }

            _context.Capacities.Add(Capacity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostCapacity), new { id = Capacity.CapacityID }, Capacity);

        }

    }
}
