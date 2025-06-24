using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StockManagement.Data;
using StockManagement.Models;
using System.Data;
using System.Text.Json;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly StockManagementContext _context;
        private readonly IConfiguration _configuration;

        public OrdersController(StockManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] JsonElement data)
        {
            try
            {
                int customerId = data.GetProperty("customerId").GetInt32();
                int statusId = data.GetProperty("statusId").GetInt32();
                decimal orderDiscount = data.GetProperty("orderDiscount").GetDecimal();
                var items = data.GetProperty("items").EnumerateArray();

                List<int> itemDetailsIds = new();
                List<int> quantities = new();
                List<decimal> discounts = new();
                List<decimal> amounts = new();

                foreach (var item in items)
                {
                    int itemDetailsId = item.GetProperty("itemDetailsId").GetInt32();
                    int quantity = item.GetProperty("quantity").GetInt32();
                    decimal discount = item.GetProperty("discount").GetDecimal();
                    decimal salePrice = item.GetProperty("salePrice").GetDecimal();

                    // Optional validation: discount must not exceed sale price
                    if (salePrice < discount)
                        throw new Exception($"Item discount cannot exceed sale price. Item ID: {itemDetailsId}");

                    itemDetailsIds.Add(itemDetailsId);
                    quantities.Add(quantity);
                    discounts.Add(discount);
                    amounts.Add(salePrice);
                }

                var itemIdsCsv = string.Join(",", itemDetailsIds);
                var quantitiesCsv = string.Join(",", quantities);
                var discountsCsv = string.Join(",", discounts);
                var amountsCsv = string.Join(",", amounts);

                int orderId = 0;

                // 🔐 Replace with actual logic to get the logged-in employee ID
                // Example: int employeeId = int.Parse(User.FindFirst("employee_id").Value);
                int employeeId = data.GetProperty("employeeId").GetInt32();

                using var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
            EXEC sp_PlaceOrder 
                @CustomerId, @StatusId, @OrderDiscount, 
                @ItemDetailsIds, @Quantities, @Discounts, @Amounts, @EmployeeId";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new SqlParameter("@CustomerId", customerId));
                cmd.Parameters.Add(new SqlParameter("@StatusId", statusId));
                cmd.Parameters.Add(new SqlParameter("@OrderDiscount", orderDiscount));
                cmd.Parameters.Add(new SqlParameter("@ItemDetailsIds", itemIdsCsv));
                cmd.Parameters.Add(new SqlParameter("@Quantities", quantitiesCsv));
                cmd.Parameters.Add(new SqlParameter("@Discounts", discountsCsv));
                cmd.Parameters.Add(new SqlParameter("@Amounts", amountsCsv));
                cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    orderId = reader.GetInt32(0);
                }

                return Ok(new
                {
                    message = "Order placed successfully",
                    orderId = orderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error placing order",
                    error = ex.Message
                });
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelOrder([FromBody] JsonElement data)
        {
            try
            {
                int orderId = data.GetProperty("orderId").GetInt32();

                using var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "EXEC sp_CancelOrder @OrderId";
                cmd.CommandType = CommandType.Text;

                var orderIdParam = new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId };
                cmd.Parameters.Add(orderIdParam);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return Ok(new
                    {
                        orderId = reader["order_id"],
                        message = reader["message"]
                    });
                }

                return BadRequest(new { message = "Order cancellation failed or order not found." });
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(new { message = sqlEx.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error cancelling order",
                    error = ex.Message
                });
            }
        }

        [HttpGet("get-order-items/{orderId}")]
        public async Task<IActionResult> GetOrderedItems(int orderId)
        {
            var result = new List<object>();
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sp_GetOrderedItemsByOrderId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new
                    {
                        orderedItemId = reader["ordered_item_id"],
                        itemDetailsId = reader["item_details_id"],
                        quantity = reader["quantity"],
                        amount = reader["amount"],
                        itemName = reader["item_name"],
                        modelNumber = reader["model_number"],
                        barcode = reader["barcode"],
                        serialNumber = reader["serial_number"],
                        imei1 = reader["imei_1"],
                        dateReceived = reader["date_received"]
                    });
                }

                if (result.Count == 0)
                    return NotFound(new { message = "No items found for this order." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to fetch order items", error = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult GetTestData()
        {
            var items = _context.Orders.ToList();
            return Ok(items);
        }
    }
}
