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

                // Execute stored procedure
                await _context.Database.ExecuteSqlRawAsync(@"
                    EXEC sp_PlaceOrder 
                        @CustomerId = {0},
                        @StatusId = {1},
                        @OrderDiscount = {2},
                        @ItemDetailsIds = {3},
                        @Quantities = {4},
                        @Discounts = {5},
                        @Amounts = {6}
                    ",
                    customerId,
                    statusId,
                    orderDiscount,
                    itemIdsCsv,
                    quantitiesCsv,
                    discountsCsv,
                    amountsCsv
                );

                return Ok(new { message = "Order placed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error placing order", error = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult GetTestData()
        {
            var items = _context.Roles.ToList();
            return Ok(items);
        }
    }
}
