using System;

namespace StockManagement.Models
{
    public class Specs
    {
        public int Id { get; set; }

        public string? Memory { get; set; }
        public string? Storage { get; set; }
        public string? ScreenSize { get; set; }

        public string? Power { get; set; }

        // Foreign key
        public int CategoryId { get; set; }

        // Navigation property
        public Category Category { get; set; }
    }
}
