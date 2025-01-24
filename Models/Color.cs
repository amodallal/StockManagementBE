using System;
using System.Collections.Generic;
namespace StockManagement.Models


{
    public class Color
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; } = null!;

        // Navigation property
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }

}
