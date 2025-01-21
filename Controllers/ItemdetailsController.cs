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
