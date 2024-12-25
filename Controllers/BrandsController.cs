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
            var items = _context.Brands.ToList();
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> PostItem([FromBody] Brand Brand)
        {
          

            _context.Brands.Add(Brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostItem), new { id = Brand.BrandId }, Brand);
        }

    }
}
