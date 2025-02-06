using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StockManagement.Models;
using System.Data;

[Route("api/controller")]
[ApiController]
public class SalesmanStockController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public SalesmanStockController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("transfer-imei-stock")]
    public async Task<IActionResult> TransferIMEIStockToSalesman([FromBody] TransferIMEIStockRequest request)
    {
        if (string.IsNullOrEmpty(request.IMEI_1) || request.Employee_id <= 0)
        {
            return BadRequest("Invalid input.");
        }

        try
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("TransferIMEIStockToSalesman", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Employee_id", request.Employee_id);
                    cmd.Parameters.AddWithValue("@imei_1", request.IMEI_1);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                        return Ok(new { message = "Stock transferred successfully." });
                    else
                        return NotFound(new { message = "IMEI not found or already transferred." });
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
        }
    }
}
