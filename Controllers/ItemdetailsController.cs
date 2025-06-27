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
            var itemDetailsQuery = _context.ItemDetails
                .Where(item => item.Imei1 == imei1)
                // LEFT JOIN to Items
                .GroupJoin(_context.Items,
                    id => id.ItemId,
                    i => i.ItemId,
                    (itemDetail, items) => new { itemDetail, items })
                .SelectMany(
                    x => x.items.DefaultIfEmpty(),
                    (x, item) => new { x.itemDetail, item })
                // LEFT JOIN to Brands
                .GroupJoin(_context.Brands,
                    prev => prev.item.BrandId,
                    b => b.BrandId,
                    (prev, brands) => new { prev.itemDetail, prev.item, brands })
                .SelectMany(
                    x => x.brands.DefaultIfEmpty(),
                    (x, brand) => new { x.itemDetail, x.item, brand })
                // LEFT JOIN to Colors
                .GroupJoin(_context.Colors,
                    prev => prev.item.ColorId,
                    c => c.ColorId,
                    (prev, colors) => new { prev.itemDetail, prev.item, prev.brand, colors })
                .SelectMany(
                    x => x.colors.DefaultIfEmpty(),
                    (x, color) => new { x.itemDetail, x.item, x.brand, color })
                // LEFT JOIN to ItemSupplier (this logic seems complex, ensuring it handles nulls)
                .GroupJoin(_context.ItemSupplier,
                    prev => prev.item.ItemId,
                    itemsup => itemsup.ItemId,
                    (prev, itemSuppliers) => new { prev.itemDetail, prev.item, prev.brand, prev.color, itemSuppliers })
                .SelectMany(
                    x => x.itemSuppliers.Where(itemsup => itemsup.SupplierId == x.itemDetail.SupplierId).DefaultIfEmpty(),
                    (x, itemSupplier) => new { x.itemDetail, x.item, x.brand, x.color, itemSupplier })
                // LEFT JOIN to Suppliers
                .GroupJoin(_context.Suppliers,
                prev => prev.itemDetail.SupplierId,
                s => s.SupplierId,
                    (prev, suppliers) => new { prev.itemDetail, prev.item, prev.brand, prev.color, prev.itemSupplier, suppliers })
                .SelectMany(
                    x => x.suppliers.DefaultIfEmpty(),
                    (x, supplier) => new { x.itemDetail, x.item, x.brand, x.color, x.itemSupplier, supplier })
                .Select(joined => new
                {
                    joined.itemDetail.ItemDetailsId,
                    joined.itemDetail.ItemId,
                    joined.itemDetail.SerialNumber,
                    joined.itemDetail.Imei1,
                    joined.itemDetail.Imei2,
                    joined.itemDetail.Barcode,
                    joined.itemDetail.Quantity,
                    joined.itemDetail.Cost,
                    joined.itemDetail.SalePrice,
                    joined.itemDetail.DateReceived,
                    // Use null-conditional operator to prevent errors if a join failed
                    SupplierName = joined.supplier != null ? joined.supplier.SupplierName : "N/A",
                    BrandName = joined.brand != null ? joined.brand.BrandName : "N/A",
                    ItemName = joined.item != null ? joined.item.Name : "N/A",
                    ModelNumber = joined.item != null ? joined.item.ModelNumber : "N/A",
                    ColorName = joined.color != null ? joined.color.ColorName : "N/A",
                    // Handling many-to-many for capacities would require a separate query or sub-select for robustness
                    // For simplicity, this part is omitted but should also be handled carefully.
                    CapacityName = joined.item.Capacities.FirstOrDefault().CapacityName ?? "N/A"
                })
                .FirstOrDefaultAsync();

            var result = await itemDetailsQuery;

            if (result == null)
            {
                return NotFound("Item not found.");
            }

            return Ok(result);
        }


        [HttpGet("GetBySerialNumber/{serialNumber}")]
        public async Task<IActionResult> GetBySerialNumber(string serialNumber)
        {
            // The query is now structured with LEFT JOINs to be more robust.
            var itemDetailsQuery = _context.ItemDetails
                .Where(item => item.SerialNumber == serialNumber)
                // LEFT JOIN to Items
                .GroupJoin(_context.Items,
                    id => id.ItemId,
                    i => i.ItemId,
                    (itemDetail, items) => new { itemDetail, items })
                .SelectMany(
                    x => x.items.DefaultIfEmpty(),
                    (x, item) => new { x.itemDetail, item })
                // LEFT JOIN to Categories
                .GroupJoin(_context.Categories,
                    prev => prev.item.CategoryId,
                    cat => cat.CategoryId,
                    (prev, categories) => new { prev.itemDetail, prev.item, categories })
                .SelectMany(
                    x => x.categories.DefaultIfEmpty(),
                    (x, category) => new { x.itemDetail, x.item, category })
                // LEFT JOIN to Brands
                .GroupJoin(_context.Brands,
                    prev => prev.item.BrandId,
                    b => b.BrandId,
                    (prev, brands) => new { prev.itemDetail, prev.item, prev.category, brands })
                .SelectMany(
                    x => x.brands.DefaultIfEmpty(),
                    (x, brand) => new { x.itemDetail, x.item, x.category, brand })
                // LEFT JOIN to ItemSupplier
                .GroupJoin(_context.ItemSupplier,
                    prev => prev.item.ItemId,
                    itemsup => itemsup.ItemId,
                    (prev, itemSuppliers) => new { prev.itemDetail, prev.item, prev.category, prev.brand, itemSuppliers })
                .SelectMany(
                    x => x.itemSuppliers.Where(itemsup => itemsup.SupplierId == x.itemDetail.SupplierId).DefaultIfEmpty(),
                    (x, itemSupplier) => new { x.itemDetail, x.item, x.category, x.brand, itemSupplier })
                // LEFT JOIN to Suppliers
                .GroupJoin(_context.Suppliers,
                    prev => prev.itemDetail.SupplierId,
                    s => s.SupplierId,
                    (prev, suppliers) => new { prev, suppliers })
                .SelectMany(
                    x => x.suppliers.DefaultIfEmpty(),
                    (x, supplier) => new { x.prev.itemDetail, x.prev.item, x.prev.category, x.prev.brand, x.prev.itemSupplier, supplier })
                // LEFT JOIN to Colors
                .GroupJoin(_context.Colors,
                    prev => prev.item.ColorId,
                    c => c.ColorId,
                    (prev, colors) => new { prev, colors })
                .SelectMany(
                    x => x.colors.DefaultIfEmpty(),
                    (x, color) => new
                    {
                        x.prev.itemDetail,
                        x.prev.item,
                        x.prev.category,
                        x.prev.brand,
                        x.prev.itemSupplier,
                        x.prev.supplier,
                        color
                    })
                .Select(joined => new
                {
                    joined.itemDetail.ItemDetailsId,
                    joined.itemDetail.ItemId,
                    joined.itemDetail.SerialNumber,
                    joined.itemDetail.Imei1,
                    joined.itemDetail.Imei2,
                    joined.itemDetail.Barcode,
                    joined.itemDetail.Quantity,
                    joined.itemDetail.SalePrice,
                    joined.itemDetail.Cost,
                    joined.itemDetail.DateReceived,
                    // Use null-conditional operators to safely access joined data
                    SupplierName = joined.supplier != null ? joined.supplier.SupplierName : "N/A",
                    BrandName = joined.brand != null ? joined.brand.BrandName : "N/A",
                    ItemName = joined.item != null ? joined.item.Name : "N/A",
                    ModelNumber = joined.item != null ? joined.item.ModelNumber : "N/A",
                    ColorName = joined.color != null ? joined.color.ColorName : "N/A",
                    Identifier = joined.category != null ? joined.category.Identifier : "N/A",
                    // Safely get the first capacity or a default value
                    CapacityName = joined.item.Capacities.FirstOrDefault() != null ? joined.item.Capacities.First().CapacityName : "N/A"
                })
                .FirstOrDefaultAsync();

            var itemDetails = await itemDetailsQuery;

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
                    itemDetail => itemDetail.ItemId,
                    item => item.ItemId,
                    (itemDetail, item) => new { itemDetail, item })
                .Join(_context.Suppliers,
                    join => join.itemDetail.SupplierId,
                    supplier => supplier.SupplierId,
                    (join, supplier) => new { join.itemDetail, join.item, supplier })
                .ToListAsync(); // Fetch all matching rows

            if (!itemDetailsList.Any())
                return NotFound("No items found for this barcode.");

            var results = itemDetailsList.Select(join =>
            {
                var brand = _context.Brands.FirstOrDefault(b => b.BrandId == join.item.BrandId);
                var color = _context.Colors.FirstOrDefault(c => c.ColorId == join.item.ColorId);
                var category = _context.Categories.FirstOrDefault(cat => cat.CategoryId == join.item.CategoryId);
                var capacity = join.item.Capacities?.FirstOrDefault();

                return new
                {
                    join.itemDetail.ItemDetailsId,
                    join.itemDetail.ItemId,
                    join.itemDetail.SerialNumber,
                    join.itemDetail.Imei1,
                    join.itemDetail.Imei2,
                    join.itemDetail.Barcode,
                    join.itemDetail.Quantity,
                    join.itemDetail.SalePrice,
                    join.itemDetail.Cost,
                    join.itemDetail.DateReceived,
                    SupplierName = join.supplier?.SupplierName ?? "N/A",
                    BrandName = brand?.BrandName ?? "N/A",
                    ItemName = join.item?.Name ?? "N/A",
                    ModelNumber = join.item?.ModelNumber ?? "N/A",
                    ColorName = color?.ColorName ?? "N/A",
                    CategoryIdentifier = category?.Identifier ?? "N/A",
                    CapacityName = capacity?.CapacityName ?? "N/A"
                };
            }).ToList();

            return Ok(results);
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
