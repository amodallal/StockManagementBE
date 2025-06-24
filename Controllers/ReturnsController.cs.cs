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
        public async Task<IActionResult> ReturnItems([FromBody] JsonElement data)
        {
            try
            {
                int orderId = data.GetProperty("orderId").GetInt32();
                string notes = data.GetProperty("notes").GetString() ?? "";

                var items = data.GetProperty("items").EnumerateArray();

                List<int> itemDetailsIds = new();
                List<int> quantities = new();
                List<int> isDamagedFlags = new();
                List<string> reasons = new();

                foreach (var item in items)
                {
                    itemDetailsIds.Add(item.GetProperty("itemDetailsId").GetInt32());
                    quantities.Add(item.GetProperty("quantity").GetInt32());
                    isDamagedFlags.Add(item.GetProperty("isDamaged").GetBoolean() ? 1 : 0);
                    reasons.Add(item.GetProperty("reason").GetString() ?? "");
                }

                string itemIdsCsv = string.Join(",", itemDetailsIds);
                string quantitiesCsv = string.Join(",", quantities);
                string isDamagedCsv = string.Join(",", isDamagedFlags);
                string reasonsCsv = string.Join(",", reasons.Select(r => r.Replace(",", ";"))); // sanitize

                await _context.Database.ExecuteSqlRawAsync(@"
            EXEC sp_ReturnItems 
                @OrderId = {0}, 
                @Notes = {1}, 
                @ItemDetailsIds = {2}, 
                @Quantities = {3}, 
                @IsDamagedFlags = {4}, 
                @Reasons = {5}
            ",
                    orderId,
                    notes,
                    itemIdsCsv,
                    quantitiesCsv,
                    isDamagedCsv,
                    reasonsCsv
                );

                return Ok(new { message = "Items returned successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


    

    }
}
