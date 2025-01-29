using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Data;
using StockManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemdetailsController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public ItemdetailsController(StockManagementContext context)
        {
            _context = context;
        }

        // Existing GET endpoint to fetch all item details
        [HttpGet]
        public IActionResult GetTestData()
        {
            var itemDetails = _context.ItemDetails.ToList();
            return Ok(itemDetails);
        }

        // POST endpoint to add new item details
        [HttpPost]
        public async Task<IActionResult> PostItemDetails([FromBody] ItemDetail itemDetail)
        {
            _context.ItemDetails.Add(itemDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostItemDetails), new { id = itemDetail.ItemDetailsId }, itemDetail);
        }

        // New endpoint to check if IMEI exists
        [HttpGet("CheckIMEI/{imei1}")]
        public async Task<IActionResult> CheckIMEIExists(string imei1)
        {
            var exists = await _context.ItemDetails
                .AnyAsync(item => item.Imei1 == imei1);

            return Ok(new { exists });
        }

        [HttpGet("GetbyIMEI/{imei1}")]
        public async Task<IActionResult> GetByIMEI(string imei1)
        {
            // Fetch item details along with supplier name, brand name, item name, capacities, color, and sale price/cost from ItemSupplier
            var itemDetails = await _context.ItemDetails
                .Where(item => item.Imei1 == imei1)
                .Join(_context.Items,
                      itemDetails => itemDetails.ItemId,
                      item => item.ItemId,
                      (itemDetails, item) => new { itemDetails, item })  // Joining ItemDetails with Items

                .Join(_context.Brands,  // Join with Brands table
                      joined => joined.item.BrandId,
                      brand => brand.BrandId,
                      (joined, brand) => new { joined.itemDetails, joined.item, brand }) // Adding Brand data

                .Join(_context.ItemSupplier,  // Join with ItemSupplier table using the ItemId and SupplierId from ItemDetails
                      joined => joined.item.ItemId,
                      itemSupplier => itemSupplier.ItemId,
                      (joined, itemSupplier) => new { joined.itemDetails, joined.item, joined.brand, itemSupplier }) // Joining with ItemSupplier
                .Where(joined => joined.itemDetails.SupplierId == joined.itemSupplier.SupplierId)  // Filter by SupplierId from ItemDetails

                .Join(_context.Suppliers,  // Join with Supplier table using SupplierId from ItemSupplier
                      joined => joined.itemSupplier.SupplierId,
                      supplier => supplier.SupplierId,
                      (joined, supplier) => new { joined.itemDetails, joined.item, joined.brand, joined.itemSupplier, supplier }) // Joining with Supplier

                .Join(_context.Colors,  // Join with Colors table using ColorId
                      joined => joined.item.ColorId,
                      color => color.ColorId,
                      (joined, color) => new { joined.itemDetails, joined.item, joined.brand, joined.itemSupplier, joined.supplier, color }) // Adding Color data

                // Use the navigation property for the many-to-many relationship with Capacities
                .SelectMany(joined => joined.item.Capacities,  // Access the Capacities navigation property
                            (joined, capacity) => new
                            {
                                joined.itemDetails.ItemDetailsId,
                                joined.itemDetails.ItemId,
                                joined.itemDetails.SerialNumber,
                                joined.itemDetails.Imei1,
                                joined.itemDetails.Imei2,
                                SalePrice = joined.itemSupplier.SalePrice,  // Get SalePrice from ItemSupplier
                                Cost = joined.itemSupplier.CostPrice,  // Get Cost from ItemSupplier
                                joined.itemDetails.DateReceived,
                                SupplierName = joined.supplier.SupplierName,  // Getting Supplier Name
                                BrandName = joined.brand.BrandName,  // Getting Brand Name from Brands table
                                ItemName = joined.item.Name,  // Fetching Item Name from Items table
                                ModelNumber = joined.item.ModelNumber,
                                CapacityName = capacity.CapacityName,  // Fetching Capacity Name from Capacities table
                                ColorName = joined.color.ColorName  // Fetching Color Name from Colors table
                            })
                .FirstOrDefaultAsync();

            // Check if the item exists
            if (itemDetails == null)
            {
                return NotFound("Item not found.");
            }

            return Ok(itemDetails);
        }



        // New POST endpoint to add item details in batches
        [HttpPost("batch")]
        public async Task<IActionResult> PostItemDetailsBatch([FromBody] List<ItemDetail> itemDetails)
        {
            if (itemDetails == null || !itemDetails.Any())
            {
                return BadRequest("The request body must contain a non-empty list of item details.");
            }

            try
            {
                // Add items in a single operation for efficiency
                await _context.ItemDetails.AddRangeAsync(itemDetails);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"{itemDetails.Count} item details added successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while processing the batch.",
                    details = ex.Message
                });
            }
        }
    }
}
