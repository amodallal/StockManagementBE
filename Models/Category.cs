using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string Identifier { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
