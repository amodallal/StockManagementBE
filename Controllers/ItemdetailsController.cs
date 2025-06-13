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


        [HttpGet("CheckSN/{SerialNumber}")]
        public async Task<IActionResult> CheckSN(string SerialNumber)
        {
            var exists = await _context.ItemDetails
                .AnyAsync(item => item.SerialNumber == SerialNumber);

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
                .Join(_context.Categories,  // Join with Category table using CategoryId
                      joined => joined.item.CategoryId,
                      category => category.CategoryId,
                      (joined, category) => new { joined.itemDetails, joined.item, category }) // Adding Category

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
                                joined.itemDetails.Barcode,
                                joined.itemDetails.Quantity,                                SalePrice = joined.itemSupplier.SalePrice,  // Get SalePrice from ItemSupplier
                                Cost = joined.itemSupplier.CostPrice,  // Get Cost from ItemSupplier
                                joined.itemDetails.DateReceived,
                                SupplierName = joined.supplier.SupplierName,  // Getting Supplier Name
                                BrandName = joined.brand.BrandName,  // Getting Brand Name from Brands table
                                ItemName = joined.item.Name,  // Fetching Item Name from Items table
                                ModelNumber = joined.item.ModelNumber,
                                CapacityName = capacity.CapacityName,  // Fetching Capacity Name from Capacities table
                                ColorName = joined.color.ColorName,  // Fetching Color Name from Colors table
                                
          
                            })
                .FirstOrDefaultAsync();

            // Check if the item exists
            if (itemDetails == null)
            {
                return NotFound("Item not found.");
            }

            return Ok(itemDetails);
        }

        [HttpGet("GetBySerialNumber/{serialNumber}")]
        public async Task<IActionResult> GetBySerialNumber(string serialNumber)
        {
            // Fetch item details along with category identifier, supplier name, brand name, item name, capacities, color, and sale price/cost from ItemSupplier
            var itemDetails = await _context.ItemDetails
                .Where(item => item.SerialNumber == serialNumber)

                .Join(_context.Items,  // Join with Items table
                      itemDetails => itemDetails.ItemId,
                      item => item.ItemId,
                      (itemDetails, item) => new { itemDetails, item })

                .Join(_context.Categories,  // Join with Category table using CategoryId
                      joined => joined.item.CategoryId,
                      category => category.CategoryId,
                      (joined, category) => new { joined.itemDetails, joined.item, category }) // Adding Category

                .Join(_context.Brands,  // Join with Brands table
                      joined => joined.item.BrandId,
                      brand => brand.BrandId,
                      (joined, brand) => new { joined.itemDetails, joined.item, joined.category, brand }) // Adding Brand

                .Join(_context.ItemSupplier,  // Join with ItemSupplier table
                      joined => joined.item.ItemId,
                      itemSupplier => itemSupplier.ItemId,
                      (joined, itemSupplier) => new { joined.itemDetails, joined.item, joined.category, joined.brand, itemSupplier })

                .Join(_context.Suppliers,  // Join with Supplier table using SupplierId from ItemSupplier
                      joined => joined.itemSupplier.SupplierId,
                      supplier => supplier.SupplierId,
                      (joined, supplier) => new { joined.itemDetails, joined.item, joined.category, joined.brand, joined.itemSupplier, supplier })

                .Join(_context.Colors,  // Join with Colors table using ColorId
                      joined => joined.item.ColorId,
                      color => color.ColorId,
                      (joined, color) => new { joined.itemDetails, joined.item, joined.category, joined.brand, joined.itemSupplier, joined.supplier, color })

                // Use navigation property for the many-to-many relationship with Capacities
                .SelectMany(joined => joined.item.Capacities,
                            (joined, capacity) => new
                            {
                                joined.itemDetails.ItemDetailsId,
                                joined.itemDetails.ItemId,
                                joined.itemDetails.SerialNumber,
                                joined.itemDetails.Imei1,
                                joined.itemDetails.Imei2,
                                joined.itemDetails.Barcode,
                                joined.itemDetails.Quantity,
                                SalePrice = joined.itemSupplier.SalePrice,
                                Cost = joined.itemSupplier.CostPrice,
                                joined.itemDetails.DateReceived,
                                SupplierName = joined.supplier.SupplierName,
                                BrandName = joined.brand.BrandName,
                                ItemName = joined.item.Name,
                                ModelNumber = joined.item.ModelNumber,
                                CapacityName = capacity.CapacityName,
                                ColorName = joined.color.ColorName,
                                Identifier = joined.category.Identifier  // Fetching Category Identifier
                            })
                .FirstOrDefaultAsync();

            // Check if the item exists
            if (itemDetails == null)
            {
                return NotFound("Item not found.");
            }

            return Ok(itemDetails);
        }


        [HttpGet("GetByBarcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var itemDetailsList = await _context.ItemDetails
                .Where(item => item.Barcode == barcode)
                .Join(_context.Items,
                      itemDetails => itemDetails.ItemId,
                      item => item.ItemId,
                      (itemDetails, item) => new { itemDetails, item }) // Joining ItemDetails with Items

                .Join(_context.Brands,  // Join with Brands table
                      joined => joined.item.BrandId,
                      brand => brand.BrandId,
                      (joined, brand) => new { joined.itemDetails, joined.item, brand }) // Adding Brand data

                .Join(_context.ItemSupplier,  // Join with ItemSupplier table
                      joined => joined.item.ItemId,
                      itemSupplier => itemSupplier.ItemId,
                      (joined, itemSupplier) => new { joined.itemDetails, joined.item, joined.brand, itemSupplier }) // Joining with ItemSupplier
                .Where(joined => joined.itemDetails.SupplierId == joined.itemSupplier.SupplierId)  // Filter by SupplierId from ItemDetails

                .Join(_context.Suppliers,  // Join with Supplier table
                      joined => joined.itemSupplier.SupplierId,
                      supplier => supplier.SupplierId,
                      (joined, supplier) => new { joined.itemDetails, joined.item, joined.brand, joined.itemSupplier, supplier }) // Joining with Supplier

                .Join(_context.Colors,  // Join with Colors table
                      joined => joined.item.ColorId,
                      color => color.ColorId,
                      (joined, color) => new { joined.itemDetails, joined.item, joined.brand, joined.itemSupplier, joined.supplier, color }) // Adding Color data

                .Join(_context.Categories,  // Join with Category table
                      joined => joined.item.CategoryId,
                      category => category.CategoryId,
                      (joined, category) => new { joined.itemDetails, joined.item, joined.brand, joined.itemSupplier, joined.supplier, joined.color, category }) // Adding Category data

                // Use the navigation property for the many-to-many relationship with Capacities
                .SelectMany(joined => joined.item.Capacities,  // Access the Capacities navigation property
                            (joined, capacity) => new
                            {
                                
                                joined.itemDetails.ItemId,
                                joined.itemDetails.SerialNumber,
                                joined.itemDetails.Imei1,
                                joined.itemDetails.Imei2,
                                joined.itemDetails.Barcode,
                                joined.itemDetails.Quantity,
                                joined.itemDetails.SalePrice,  // Get SalePrice from ItemSupplier
                                joined.itemDetails.Cost, 
                                joined.itemDetails.DateReceived,
                                SupplierName = joined.supplier.SupplierName,  // Getting Supplier Name
                                BrandName = joined.brand.BrandName,  // Getting Brand Name from Brands table
                                ItemName = joined.item.Name,  // Fetching Item Name from Items table
                                ModelNumber = joined.item.ModelNumber,
                                CapacityName = capacity.CapacityName,  // Fetching Capacity Name from Capacities table
                                ColorName = joined.color.ColorName,  // Fetching Color Name from Colors table
                                CategoryIdentifier = joined.category.Identifier  // Fetching Category Identifier from Category table
                            })
                .ToListAsync();

            // Check if any items exist
            if (itemDetailsList == null || itemDetailsList.Count == 0)
            {
                return NotFound(new { message = "No items found for this barcode." });
            }

            return Ok(itemDetailsList);
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
