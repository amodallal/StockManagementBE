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
    public class CapacitiesController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public CapacitiesController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/brand
        [HttpGet]
        public IActionResult GetTestData()
        {
            var items = _context.Capacities.ToList();
            return Ok(items);
        }
    }
}
