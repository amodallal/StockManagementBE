﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Data;
using StockManagement.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesmanController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public SalesmanController(StockManagementContext context)
        {
            _context = context;
        }

        // GET : api/salesman/{employeeId}/items  => get all transferred items 
        [HttpGet("{employeeId}/items")]
        public async Task<ActionResult<IEnumerable<object>>> GetItemsBySalesmanId(int employeeId)
        {
            var salesmanStock = await _context.SalesmanStocks
        .Where(ss => ss.EmployeeId == employeeId)
        .Include(ss => ss.Status) // Join with the Status table
        .Include(ss => ss.Item) // Join with Item
        .ThenInclude(i => i.Capacities) // Join with Capacities through Many-to-Many
        .Include(ss => ss.Item.Category) // Include Category (join with Category table)
        .Where(ss => ss.Status.StatusName == "transferred") // Filter by StatusId = 'transferred'
        .Select(ss => new
        {
            Imei1 = ss.Imei1,
            ItemName = ss.Item.Name,
            ModelNumber = ss.Item.ModelNumber,
            Color = _context.Colors // Ensure we select only the color name
                .Where(c => c.ColorId == ss.Item.ColorId)
                .Select(c => c.ColorName)
                .FirstOrDefault() ?? "Unknown", // Handle null values
            Capacities = ss.Item.Capacities.Select(c => c.CapacityName).ToList(),
            Quantity = ss.Quantity ?? 0, // Include Quantity
            Cost = ss.Cost ?? 0, // Include Cost
            SalePrice = ss.SalePrice ?? 0, // Include Sale Price
            Category = ss.Item.Category.CategoryName, // Include Category Name
            StatusName = ss.Status.StatusName, // Include Status Name (from the Status entity)
            Description = ss.Item.Description // Include Item Description
        }).ToListAsync();

            if (!salesmanStock.Any())
            {
                return NotFound("No items found for this salesman.");
            }

            return Ok(salesmanStock);
        }
    }
}