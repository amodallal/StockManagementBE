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
    public class ItemsController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public ItemsController(StockManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            return await _context.Items.ToListAsync();
        }
        [HttpPost]
        public async Task<IActionResult> PostItem([FromBody] Item item)
        {
            if (item == null)
            {
                return BadRequest("Item is null");
            }

            // Check if required fields are missing
            if (string.IsNullOrEmpty(item.Name))
            {
                return BadRequest("Item name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(item.ModelNumber))
            {
                return BadRequest("Item model cannot be null or empty");
            }



            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostItem), new { id = item.ItemId }, item);
        }


    }
}
