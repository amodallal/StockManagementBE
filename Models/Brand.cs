using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
