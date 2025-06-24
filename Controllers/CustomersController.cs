using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Data;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public CustomersController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _context.Customers
                .Select(c => new
                {
                    c.CustomerId,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.PhoneNumber,
                    c.Address
                })
                .ToListAsync();

            return Ok(customers);
        }
    }
}
