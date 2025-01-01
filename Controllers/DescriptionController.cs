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
    public class DescriptionController : ControllerBase
    {
        private readonly StockManagementContext _context;


        public DescriptionController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/Description
        [HttpGet]
        public IActionResult GetTestData()
        {
            var Description = _context.Descriptions.ToList();
            return Ok(Description);
        }

        
    }
}
