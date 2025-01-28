using Microsoft.AspNetCore.Mvc;
using StockManagement.Data;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public StatusController(StockManagementContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetTestData()
        {
            var Statuses = _context.Statuses.ToList();
            return Ok(Statuses);
        }


    }
}
