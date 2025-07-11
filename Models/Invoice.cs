using System.Collections.Generic;
namespace StockManagement.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public Customer Customer { get; set; }
    }

}
