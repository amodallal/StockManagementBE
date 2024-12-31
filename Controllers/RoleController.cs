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
    public class RoleController : ControllerBase
    {
        private readonly StockManagementContext _context;
  
        
            public RoleController(StockManagementContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetTestData()
        {
            var items = _context.Roles.ToList();
            return Ok(items);
        }

    }
}
