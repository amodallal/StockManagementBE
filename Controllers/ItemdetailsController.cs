using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Data;
using StockManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
namespace StockManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemdetailsController : ControllerBase
    {
        private readonly StockManagementContext _context;

        public ItemdetailsController(StockManagementContext context)
        {
            _context = context;
        }


        // GET: api/ItemDetail
        [HttpGet]
        public IActionResult GetTestData()
        {
            var Itemdetails = _context.ItemDetails.ToList();
            return Ok(Itemdetails);
        }





        [HttpPost]
        public async Task<IActionResult> PostItemDetails([FromBody] ItemDetail itemDetail)
        {



            _context.ItemDetails.Add(itemDetail);
            await _context.SaveChangesAsync();

            // Use CreatedAtAction to return a 201 Created status with the location of the new resource
            return CreatedAtAction(nameof(PostItemDetails), new { id = itemDetail.ItemDetailsId }, itemDetail);

        }



    }
}
