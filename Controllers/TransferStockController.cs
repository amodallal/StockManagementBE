using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StockManagement.Models;
using System.Data;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class TransferStockController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public TransferStockController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Existing method for IMEI-based stock transfer
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
                    cmd.Parameters.AddWithValue("source", request.Source);
                    cmd.Parameters.AddWithValue("destination", request.Destination);
                    

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




    [HttpPost("transfer-serial-stock")]
    public async Task<IActionResult> TransferSerialStockToSalesman([FromBody] TransferSerialStockRequest request)
    {
        if (string.IsNullOrEmpty(request.SerialNumber) || request.Employee_id <= 0)
        {
            return BadRequest("Invalid input.");
        }

        try
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("TransferSerialStockToSalesman", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Employee_id", request.Employee_id);
                    cmd.Parameters.AddWithValue("@serial_number", request.SerialNumber);
                    cmd.Parameters.AddWithValue("@source", request.Source);
                    cmd.Parameters.AddWithValue("@destination", request.Destination);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                        return Ok(new { message = "Stock transferred successfully." });
                    else
                        return NotFound(new { message = "Serial number not found or already transferred." });
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
        }
    }


    // New method for quantity-based stock transfer
    [HttpPost("transfer-stock")]
    public async Task<IActionResult> TransferStockToSalesman([FromBody] TransferStockRequest request)
    {
        if (request.Employee_id <= 0 || request.ItemId <= 0 || request.TransferQuantity <= 0)
        {
            return BadRequest(new { message = "Invalid input. Please check the provided data." });
        }

        try
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("TransferStockToSalesman", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Employee_id", request.Employee_id);
                    cmd.Parameters.AddWithValue("@item_id", request.ItemId);
                    cmd.Parameters.AddWithValue("@transfer_quantity", request.TransferQuantity);
                    cmd.Parameters.AddWithValue("@source", request.Source);
                    cmd.Parameters.AddWithValue("@destination", request.Destination);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var transferredStock = new
                            {
                                SalesmanStockId = reader["SalesmanStockId"],
                                Barcode = reader["Barcode"],
                                Cost = reader["Cost"],
                                SalePrice = reader["SalePrice"],
                                Quantity = reader["Quantity"],
                                DateReceived = reader["DateReceived"],
                                StatusId = reader["StatusId"]
                            };

                            return Ok(new { message = "Stock transferred successfully.", stock = transferredStock });
                        }
                        else
                        {
                            return NotFound(new { message = "Stock transfer failed. Check item availability." });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
        }
    }

}
