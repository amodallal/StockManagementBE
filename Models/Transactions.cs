using System.Collections.Generic;
namespace StockManagement.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public char TransactionType { get; set; }

        public int? OrderId { get; set; }
        public DateTime Date { get; set; }

        public string Notes { get; set; }  

        // Navigation property
        public Customer Customer { get; set; }
        public Order Order { get; set; }   // ✅ New navigation property
    }

}
