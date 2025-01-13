using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Data;
using StockManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public BrandsController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/brand
        [HttpGet]
        public IActionResult GetTestData()
        {
            var Brands = _context.Brands.ToList();
            return Ok(Brands);
        }

        // POST: api/brands (Add a new brand)
        [HttpPost]
        public async Task<IActionResult> PostBrand([FromBody] Brand Brand)
        {
            if (Brand == null)
            {
                return BadRequest("Brand is null");
            }

            // Check if required fields are missing
            if (string.IsNullOrEmpty(Brand.BrandName))
            {
                return BadRequest("Brand name cannot be null or empty");
            }

            _context.Brands.Add(Brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostBrand), new { id = Brand.BrandId }, Brand);
        }

        // POST: api/brands (Bulk Insert)
        [HttpPost("bulk")]
        public async Task<IActionResult> AddBrands([FromBody] List<Brand> brands)
        {
            if (brands == null || !brands.Any())
            {
                return BadRequest("Brand list cannot be empty.");
            }

            try
            {
                // Check for duplicate brands in the list
                var existingBrands = await _context.Brands
                    .Where(b => brands.Select(brand => brand.BrandName.ToLower()).Contains(b.BrandName.ToLower()))
                    .ToListAsync();

                // If any brand from the request already exists, return a conflict
                if (existingBrands.Any())
                {
                    return Conflict("One or more of the provided brands already exist.");
                }

                // Add all the new brands to the database
                _context.Brands.AddRange(brands);
                await _context.SaveChangesAsync();

                // Return the newly added brands (success)
                return Ok(brands);  // Respond with 200 OK and the list of added brands
            }
            catch (Exception ex)
            {
                // Log the error and return a general error response
                Console.Error.WriteLine($"Error adding brands: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        // DELETE: api/brands/{id} (Delete a brand by BrandId)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            // Find the brand by its ID
            var brand = await _context.Brands.FindAsync(id);

            // If the brand is not found, return a NotFound status
            if (brand == null)
            {
                return NotFound("Brand not found.");
            }

            // Delete the brand from the database
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            // Return a success message
            return Ok(new { message = "Brand deleted successfully." });
        }
    }
}
