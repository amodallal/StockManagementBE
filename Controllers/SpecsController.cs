using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Data;
using StockManagement.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecsController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public SpecsController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/specs/getbycategory?categoryId=1
        [HttpGet("GetByCategory")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var specs = await _context.Specs
                
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();

            return Ok(specs);
        }
    }
}
