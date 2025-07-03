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
                i.ColorId,
                i.Description,
                CategoryName = i.Category != null ? i.Category.CategoryName : null,
                Identifier = i.Category != null ? i.Category.Identifier : null,
                BrandName = i.Brand != null ? i.Brand.BrandName : null,
                i.ItemDetails,
                i.SalesmanStocks,
                i.Barcode,
                i.SpecsId
            });

            return Ok(result);

            //return await _context.Items.ToListAsync();
        }
        // GET: api/items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            var item = await _context.Items

        .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // GET: api/items/by-model-number/{modelNumber}
        [HttpGet("bymodelnumber/{modelNumber}")]
        public async Task<ActionResult<Item>> GetItemByModelNumber(string modelNumber)
        {
            // Fetch the item by modelNumber
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.ModelNumber == modelNumber);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpGet("GetFilteredItemsNameModel")]
        public IActionResult GetFilteredItems(string search)
        {
            var items = _context.Items
                .Include(i => i.Category)
                .Include(i => i.Brand)
                .Where(i => i.Name.Contains(search) || i.ModelNumber.Contains(search))
                .Take(20)
                .ToList();

            var result = items.Select(i => new
            {
                i.ItemId,
                i.Name,
                i.ModelNumber,
                i.BrandId,
                i.CategoryId,
                i.ColorId,
                i.Description,
                CategoryName = i.Category != null ? i.Category.CategoryName : null,
                Identifier = i.Category != null ? i.Category.Identifier : null,
                BrandName = i.Brand != null ? i.Brand.BrandName : null,
                i.ItemDetails,
                i.SalesmanStocks,
                i.Barcode,
                i.SpecsId
            });

            return Ok(result);
        }



        [HttpGet("GetFilteredItems")]
        public IActionResult GetFilteredItems(string search)
        {
            var items = _context.Items
                .Include(i => i.Category) // include Category to get identifier
                .Include(i => i.Brand)
                .Where(i => i.Name.Contains(search))
                .Take(20)
                .ToList();

            var result = items.Select(i => new
            {
                i.ItemId,
                i.Name,
                i.ModelNumber,
                i.BrandId,
                i.CategoryId,
                i.ColorId,
                i.Description,
                CategoryName = i.Category != null ? i.Category.CategoryName : null,
                Identifier = i.Category != null ? i.Category.Identifier : null,
                BrandName = i.Brand != null ? i.Brand.BrandName : null,
                i.ItemDetails,
                i.SalesmanStocks,
                i.Barcode,
                i.SpecsId
            });

            return Ok(result);
        }
        // GET: api/items/GetItemspagination
        /* [HttpGet("GetItemspagination")]
         public async Task<IActionResult> GetItemspagination(
     [FromQuery] string search = "",
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 1)
         {
             var query = _context.Items
                 .AsNoTracking()
                 .Include(i => i.Specs) // ✅ Include Specs table
                 .Select(i => new
                 {
                     i.ItemId,
                     i.Name,
                     i.ModelNumber,
                     i.BrandId,
                     i.CategoryId,
                     i.ColorId,
                     i.Description,
                     i.Barcode,
                     i.SpecsId,
                     Spec = i.Specs == null ? null : new
                     {
                         i.Specs.Id,
                         i.Specs.Memory,
                         i.Specs.Storage,
                         i.Specs.ScreenSize,
                         i.Specs.Power
                     }
                 });

             if (!string.IsNullOrEmpty(search))
             {
                 query = query.Where(i =>
                     i.Name.Contains(search) ||
                     i.ModelNumber.Contains(search) ||
                     i.Description.Contains(search));
             }

             var totalItems = await query.CountAsync();
             var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

             var items = await query
                 .OrderByDescending(i => i.ItemId)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

             var result = new
             {
                 totalItems,
                 page = pageNumber,
                 pageSize,
                 totalPages,
                 items
             };

             return Ok(result);
         }

         */

        //sorted pagination
        [HttpGet("GetItemspagination")]
        public async Task<IActionResult> GetItemspagination(
    [FromQuery] string search = "",
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 1,
    [FromQuery] string sortBy = "ItemId",
    [FromQuery] bool isDesc = true)
        {
            var query = _context.Items
                .AsNoTracking()
                .Include(i => i.Specs)
                .Select(i => new
                {
                    i.ItemId,
                    i.Name,
                    i.ModelNumber,
                    i.BrandId,
                    i.CategoryId,
                    i.ColorId,
                    i.Description,
                    i.Barcode,
                    i.SpecsId,
                    Spec = i.Specs == null ? null : new
                    {
                        i.Specs.Id,
                        i.Specs.Memory,
                        i.Specs.Storage,
                        i.Specs.ScreenSize,
                        i.Specs.Power
                    }
                });

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i =>
                    i.Name.Contains(search) ||
                    i.ModelNumber.Contains(search) ||
                    i.Description.Contains(search));
            }

            // Apply dynamic sorting
            query = (sortBy.ToLower(), isDesc) switch
            {
                ("name", true) => query.OrderByDescending(i => i.Name),
                ("name", false) => query.OrderBy(i => i.Name),

                ("modelnumber", true) => query.OrderByDescending(i => i.ModelNumber),
                ("modelnumber", false) => query.OrderBy(i => i.ModelNumber),

                ("itemid", true) => query.OrderByDescending(i => i.ItemId),
                ("itemid", false) => query.OrderBy(i => i.ItemId),

                _ => query.OrderByDescending(i => i.ItemId) // default
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                totalItems,
                page = pageNumber,
                pageSize,
                totalPages,
                items
            };

            return Ok(result);
        }



        // POST: api/items
        [HttpPost]
        public async Task<IActionResult> PostItem([FromBody] Item item)
        {
            if (item == null)
                return BadRequest("Item is null");

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item);
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


        [HttpPost("supplier-item")]
        public async Task<IActionResult> AddItemSupplier([FromBody] ItemSupplier itemSupplier)
        {
            if (itemSupplier == null)
            {
                return BadRequest("Invalid data.");
            }

            // Validate if ItemId and SupplierId exist
            var itemExists = await _context.Items.AnyAsync(i => i.ItemId == itemSupplier.ItemId);
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierId == itemSupplier.SupplierId);

            if (!itemExists || !supplierExists)
            {
                return NotFound("Item or Supplier not found.");
            }

            // Check if this item-supplier entry already exists
            var existingEntry = await _context.ItemSupplier
                .FirstOrDefaultAsync(i => i.ItemId == itemSupplier.ItemId && i.SupplierId == itemSupplier.SupplierId);

            if (existingEntry != null)
            {
                return Conflict("This supplier already has this item.");
            }
            _context.ItemSupplier.Add(itemSupplier);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Item-Supplier relationship added successfully!" });
        }
        [HttpGet("supplier-item")]
        public async Task<IActionResult> GetItemSuppliers([FromQuery] int? itemId, [FromQuery] int? supplierId)
        {
            // Base query for ItemSupplier
            var query = _context.ItemSupplier.AsQueryable();

            // Filter by ItemId if provided
            if (itemId.HasValue)
            {
                query = query.Where(x => x.ItemId == itemId.Value);
            }

            // Filter by SupplierId if provided
            if (supplierId.HasValue)
            {
                query = query.Where(x => x.SupplierId == supplierId.Value);
            }

            // Fetch the filtered or all results
            var itemSuppliers = await query
                .Select(x => new
                {
                    x.ItemId,
                    x.SupplierId,
                    x.CostPrice,
                    x.SalePrice
                })
                .ToListAsync();

            // If no records found
            //if (!itemSuppliers.Any())
           // {
            //    return NotFound("No Item-Supplier relationships found.");
           // }

            return Ok(itemSuppliers);
        }
        [HttpGet("get-identifier/{barcode}")]
        public async Task<IActionResult> GetIdentifierByBarcode(string barcode)
        {
            try
            {
                var result = await (from itemDetails in _context.ItemDetails
                                    join item in _context.Items on itemDetails.ItemId equals item.ItemId
                                    join category in _context.Categories on item.CategoryId equals category.CategoryId
                                    where (itemDetails.Barcode == barcode || itemDetails.Imei1 == barcode || itemDetails.SerialNumber == barcode)
                                    select new
                                    {
                                        item.Name,
                                        itemDetails.ItemId,  // Fetching ItemId from ItemDetails
                                        category.Identifier  // Fetching Identifier from Categories via Items
                                    })
                                   .FirstOrDefaultAsync();

                if (result == null)
                {
                    return NotFound(new { message = "Identifier or ItemId not found for this barcode." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
            }
        }





    }



}

