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
    public class SupplierController : ControllerBase
    {
        private readonly StockManagementContext _context;


        public SupplierController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/Supplier
         [HttpGet]
        public IActionResult GetTestData()
        {
            var Suppliers = _context.Suppliers.ToList();
            return Ok(Suppliers);
        }

        [HttpGet("GetFilteredSuppliers")]
        public IActionResult GetFilteredSuppliers(string search)
        {
            var suppliers = _context.Suppliers
                .Where(s => s.SupplierName.Contains(search))
                .Take(20)
                .ToList();

            return Ok(suppliers);
        }


    }
}
