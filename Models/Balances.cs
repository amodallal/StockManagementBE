using System;
using System.Collections.Generic;

namespace StockManagement.Models;

    public class Balances
    {
        public int BalanceId { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public decimal BalanceAmount { get; set; }

        // Navigation property
        public Customer Customer { get; set; }
    }

