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
            // Fetch items and include the related Category data
            var items = await _context.Items
                .Include(i => i.Category) // Load the related Category data
                .Include(i => i.Brand)
                .ToListAsync();

            // Project the data to include CategoryName in the response
            var result = items.Select(i => new
            {
                i.ItemId,
                i.Name,
                i.ModelNumber,
                i.BrandId,
                i.CategoryId,
                i.Barcode,
                CategoryName = i.Category != null ? i.Category.CategoryName : null,
                BrandName = i.Brand != null ? i.Brand.BrandName : null,
                i.ItemDetails,
                i.SalesmanStocks
            });

            return Ok(result);

            //return await _context.Items.ToListAsync();
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
