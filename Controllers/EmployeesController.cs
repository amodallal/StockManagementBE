using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Data;
using StockManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Text.Json;

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
                    e.PhoneNumber,
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement data)
        {
            string username = data.GetProperty("username").GetString();
            string password = data.GetProperty("password").GetString();

            using var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
        SELECT e.employee_id, e.first_name, e.last_name, e.role_id, e.IsActive, l.password_hash
        FROM employee_logins l
        JOIN employees e ON e.employee_id = l.employee_id
        WHERE l.username = @username";
            cmd.Parameters.Add(new SqlParameter("@username", username));

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return Unauthorized(new { message = "Invalid username or password" });

            var hash = reader.GetString(reader.GetOrdinal("password_hash"));
            var isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));

            if (!isActive)
                return Unauthorized(new { message = "Account is inactive" });

            if (!BCrypt.Net.BCrypt.Verify(password, hash))
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new
            {
                employeeId = reader.GetInt32(reader.GetOrdinal("employee_id")),
                firstName = reader.GetString(reader.GetOrdinal("first_name")),
                lastName = reader.GetString(reader.GetOrdinal("last_name")),
                roleId = reader.GetInt32(reader.GetOrdinal("role_id"))
            });
        }

        [HttpPost("create-with-login")]
        public async Task<IActionResult> CreateWithLogin([FromBody] JsonElement data)
        {
            string firstName = data.GetProperty("firstName").GetString();
            string lastName = data.GetProperty("lastName").GetString();
            string phoneNumber = data.GetProperty("phoneNumber").GetString();
            int roleId = int.Parse(data.GetProperty("roleId").GetString());
            string username = data.GetProperty("username").GetString();
            string password = data.GetProperty("password").GetString();
            bool isActive = data.GetProperty("isActive").GetBoolean();

            string hashed = BCrypt.Net.BCrypt.HashPassword(password);

            using var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var tx = conn.BeginTransaction();

            try
            {
                int employeeId;

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
                INSERT INTO employees (first_name, last_name, phone_number, role_id, IsActive)
                VALUES (@firstName, @lastName, @phone, @role, @active);
                SELECT SCOPE_IDENTITY();";
                    cmd.Parameters.Add(new SqlParameter("@firstName", firstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", lastName));
                    cmd.Parameters.Add(new SqlParameter("@phone", phoneNumber));
                    cmd.Parameters.Add(new SqlParameter("@role", roleId));
                    cmd.Parameters.Add(new SqlParameter("@active", isActive));

                    employeeId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
                INSERT INTO employee_logins (employee_id, username, password_hash)
                VALUES (@eid, @username, @hash)";
                    cmd.Parameters.Add(new SqlParameter("@eid", employeeId));
                    cmd.Parameters.Add(new SqlParameter("@username", username));
                    cmd.Parameters.Add(new SqlParameter("@hash", hashed));

                    await cmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();

                return Ok(new { employeeId, firstName, lastName, phoneNumber, roleId, isActive });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest(new { message = "Failed to create employee", error = ex.Message });
            }
        }
        [HttpPut("{employeeId}/reset-password")]
        public async Task<IActionResult> ResetPassword(int employeeId, [FromBody] JsonElement data)
        {
            if (!data.TryGetProperty("password", out var passwordProp))
                return BadRequest(new { message = "Password is required." });

            var newPassword = passwordProp.GetString();
            if (string.IsNullOrWhiteSpace(newPassword))
                return BadRequest(new { message = "Invalid password." });

            var hashed = BCrypt.Net.BCrypt.HashPassword(newPassword);

            using var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE employee_logins SET password_hash = @hash WHERE employee_id = @id";
            cmd.Parameters.Add(new SqlParameter("@hash", hashed));
            cmd.Parameters.Add(new SqlParameter("@id", employeeId));

            var affected = await cmd.ExecuteNonQueryAsync();
            if (affected == 0)
                return NotFound(new { message = "Employee login not found." });

            return Ok(new { message = "Password reset successful." });
        }

    }
}
