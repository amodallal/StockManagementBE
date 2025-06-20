using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StockManagement.Data;
using System.Data;
using System.Text.Json;

namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public OrdersController(StockManagementContext context)
        {
            _context = context;
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
                    itemDetailsIds.Add(item.GetProperty("itemDetailsId").GetInt32());
                    quantities.Add(item.GetProperty("quantity").GetInt32());
                    discounts.Add(item.GetProperty("discount").GetDecimal());
                    amounts.Add(item.GetProperty("salePrice").GetDecimal());
                }

                var itemIdsCsv = string.Join(",", itemDetailsIds);
                var quantitiesCsv = string.Join(",", quantities);
                var discountsCsv = string.Join(",", discounts);
                var amountsCsv = string.Join(",", amounts);

                int orderId = 0;

                // Use ADO.NET to read the returned @OrderId from SELECT
                using var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
            EXEC sp_PlaceOrder 
                @CustomerId, @StatusId, @OrderDiscount, 
                @ItemDetailsIds, @Quantities, @Discounts, @Amounts";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new SqlParameter("@CustomerId", customerId));
                cmd.Parameters.Add(new SqlParameter("@StatusId", statusId));
                cmd.Parameters.Add(new SqlParameter("@OrderDiscount", orderDiscount));
                cmd.Parameters.Add(new SqlParameter("@ItemDetailsIds", itemIdsCsv));
                cmd.Parameters.Add(new SqlParameter("@Quantities", quantitiesCsv));
                cmd.Parameters.Add(new SqlParameter("@Discounts", discountsCsv));
                cmd.Parameters.Add(new SqlParameter("@Amounts", amountsCsv));

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
                return BadRequest(new { message = "Error placing order", error = ex.Message });
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
                // This captures RAISERROR messages from your SP
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


        [HttpGet("test")]
        public IActionResult GetTestData()
        {
            var items = _context.Orders.ToList();
            return Ok(items);
        }
    }
}
