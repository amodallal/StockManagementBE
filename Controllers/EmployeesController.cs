using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Data;
using StockManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public EmployeesController(StockManagementContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public IActionResult GetTestData()
        {
            var items = _context.Employees.ToList();
            return Ok(items);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<IActionResult> PostEmployee([FromBody] Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostEmployee), new { id = employee.EmployeeId }, employee);
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee deleted successfully." });
        }

        // PUT: api/employees/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateEmployeeStatus(int id, [FromBody] bool isActive)
        {
            // Find the employee by ID
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            // Update the employee's status
            employee.IsActive = isActive;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated employee data
            return Ok(employee);
        }
    }
}
