using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StockManagement.Data;
using System.Data;
using System.Text.Json;
using System; // Required for Exception and DBNull
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnsController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public ReturnsController(StockManagementContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReturn([FromBody] JsonElement data)
        {
            try
            {
                // --- 1. Manually parse the incoming JSON with validation ---
                if (!data.TryGetProperty("orderId", out var orderIdElement) || orderIdElement.ValueKind != JsonValueKind.Number)
                {
                    return BadRequest(new { message = "OrderId is missing or invalid." });
                }
                int orderId = orderIdElement.GetInt32();

                string notes = data.TryGetProperty("notes", out var notesElement) && notesElement.ValueKind == JsonValueKind.String
                    ? notesElement.GetString()
                    : "";

                if (!data.TryGetProperty("items", out var itemsElement) || itemsElement.ValueKind != JsonValueKind.Array)
                {
                    return BadRequest(new { message = "Items array is missing or invalid." });
                }

                // --- 2. Extract item data from the JSON array ---
                var itemsToReturn = new List<dynamic>();
                foreach (var item in itemsElement.EnumerateArray())
                {
                    itemsToReturn.Add(new
                    {
                        ItemDetailsId = item.GetProperty("itemDetailsId").GetInt32(),
                        Quantity = item.GetProperty("quantity").GetInt32(),
                        IsDamaged = item.GetProperty("isDamaged").GetBoolean(),
                        Reason = item.TryGetProperty("reason", out var reasonElement) && reasonElement.ValueKind == JsonValueKind.String
                                 ? reasonElement.GetString()
                                 : "N/A"
                    });
                }

                if (!itemsToReturn.Any())
                {
                    return BadRequest(new { message = "No items provided for return." });
                }

                // --- 3. Format data for the stored procedure ---
                var idsCsv = string.Join(",", itemsToReturn.Select(i => i.ItemDetailsId.ToString()));
                var qtyCsv = string.Join(",", itemsToReturn.Select(i => i.Quantity.ToString()));
                var damagedCsv = string.Join(",", itemsToReturn.Select(i => ((bool)i.IsDamaged ? "1" : "0")));
                var reasonsCsv = string.Join(",", itemsToReturn.Select(i => (i.Reason ?? "N/A").ToString().Replace(",", ";")));

                // --- 4. Set up parameters using the OUTPUT parameter pattern for safety ---
                var returnIdParam = new SqlParameter("@ReturnId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                // --- FIX: Explicitly define SqlDbType and Size for NVARCHAR parameters ---
                // This is the critical fix to prevent string truncation errors. Size = -1 indicates MAX.
                var parameters = new[]
                {
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = !string.IsNullOrEmpty(notes) ? notes : (object)DBNull.Value },
                    new SqlParameter("@ItemDetailsIds", SqlDbType.NVarChar, -1) { Value = idsCsv },
                    new SqlParameter("@Quantities", SqlDbType.NVarChar, -1) { Value = qtyCsv },
                    new SqlParameter("@IsDamagedFlags", SqlDbType.NVarChar, -1) { Value = damagedCsv },
                    new SqlParameter("@Reasons", SqlDbType.NVarChar, -1) { Value = reasonsCsv },
                    returnIdParam
                };

                // --- 5. Execute the stored procedure using EF Core's recommended method ---
                // Using ExecuteSqlRawAsync is safer and integrates better with EF Core than manual connection handling.
                // Assuming the SP name is "sp_CreateReturn" and it uses an OUTPUT parameter as per best practice.
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_CreateReturn @OrderId, @Notes, @ItemDetailsIds, @Quantities, @IsDamagedFlags, @Reasons, @ReturnId OUT",
                    parameters
                );

                // Safely read the output parameter's value
                var returnId = (returnIdParam.Value != DBNull.Value && returnIdParam.Value != null) ? (int)returnIdParam.Value : 0;

                if (returnId > 0)
                {
                    return Ok(new
                    {
                        returnId = returnId,
                        message = "Return recorded successfully."
                    });
                }
                else
                {
                    // This case happens if the SP completes but doesn't set the output ID (e.g., transaction rollback).
                    return BadRequest(new { message = "Return could not be recorded. The stored procedure did not return a valid ID." });
                }
            }
            catch (Exception ex)
            {
                // Catch any exception, log it (important for debugging), and return a 500 error.
                // In your real application, you should use a logging framework here.
                Console.WriteLine($"An error occurred in CreateReturn: {ex}");

                return StatusCode(500, new
                {
                    message = "An internal server error occurred while processing the return.",
                    // Provide the actual error message for debugging purposes.
                    // In a production environment, you might want to hide this detailed message.
                    error = ex.Message
                });
            }
        }
    }
}
