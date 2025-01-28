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

        [HttpGet]
        public IActionResult GetEmployeesWithRoles()
        {
            var employeesWithRoles = _context.Employees
                .Include(e => e.Role) // Assuming a navigation property 'Role' exists in Employee model
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.IsActive,
                    RoleName = e.Role.RoleName // Assuming the Role model has a 'Name' property
                })
                .ToList();

            return Ok(employeesWithRoles);
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
