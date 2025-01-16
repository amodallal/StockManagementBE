using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Data;
using StockManagement.Models;
using Microsoft.EntityFrameworkCore;


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

        [HttpPost("bulk")]
        public async Task<IActionResult> AddItemsBulk([FromBody] List<Item> items)
        {
            if (items == null || !items.Any())
            {
                return BadRequest("No items provided.");
            }

            foreach (var item in items)
            {
                // Validate each item
                if (string.IsNullOrEmpty(item.Name))
                {
                    return BadRequest("One or more items have a missing or invalid name.");
                }

                if (string.IsNullOrEmpty(item.ModelNumber))
                {
                    return BadRequest("One or more items have a missing or invalid model number.");
                }
            }

            try
            {
                _context.Items.AddRange(items);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Items added successfully!", Count = items.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }
        
            // DELETE: api/items/{id}
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteItem(int id)
            {
                var Items = await _context.Items.FindAsync(id);
                if (Items == null)
                {
                    return NotFound("Item not found");
                }

                _context.Items.Remove(Items);
                await _context.SaveChangesAsync();

                return NoContent(); // 204 No Content status
            }
        [HttpPost("item-capacities")]
        public async Task<IActionResult> AddItemCapacities([FromBody] AddItemCapacitiesRequest request)
        {
            // Validation: Ensure request is not null and contains valid data
            if (request == null || request.ItemId <= 0 || request.CapacityIds == null || !request.CapacityIds.Any())
            {
                return BadRequest("Invalid input. ItemId and CapacityIds are required.");
            }

            // Fetch the item from the database to ensure it exists
            var item = await _context.Items.Include(i => i.Capacities)  // Include Capacities to handle the navigation
                                           .FirstOrDefaultAsync(i => i.ItemId == request.ItemId);
            if (item == null)
            {
                return NotFound("Item not found.");
            }

            // Fetch the capacities from the database
            var capacities = await _context.Capacities
                                           .Where(c => request.CapacityIds.Contains(c.CapacityID))
                                           .ToListAsync();

            // Check if all the provided capacities exist
            if (capacities.Count != request.CapacityIds.Count)
            {
                return NotFound("One or more capacities not found.");
            }

            // Add the new capacities to the item's Capacities collection (using Add for each capacity)
            foreach (var capacity in capacities)
            {
                item.Capacities.Add(capacity); // Use Add() instead of AddRange()
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success message if the operation was successful
            return Ok(new { message = "Item successfully associated with capacities." });
        }

         [HttpGet("item-capacities")]
         public async Task<IActionResult> GetAllItemsWithCapacities()
         {
             try
             {
                 // Fetch all items with their capacities
                 var items = await _context.Items
                     .Include(i => i.Capacities)
                     .ToListAsync();

                 // Map the result to a simplified DTO
                 var result = items.Select(item => new
                 {
                     ItemId = item.ItemId,
                     Name = item.Name,
                     ModelNumber = item.ModelNumber,
                     Capacities = item.Capacities.Select(c => new
                     {
                         c.CapacityID,
                         c.CapacityName
                     })
                 });

                 return Ok(result);
             }
             catch (Exception ex)
             {
                 return StatusCode(500, $"Internal server error: {ex.Message}");
             }
         }
      
       
        
    }



}

