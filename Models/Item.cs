using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public string? ModelNumber { get; set; }

    public int? BrandId { get; set; }

    public int CategoryId { get; set; }

    public string Barcode { get; set; } = null!;
    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; } = null!;
    public virtual ICollection<ItemDetail> ItemDetails { get; set; } = new List<ItemDetail>();

    public virtual ICollection<SalesmanStock> SalesmanStocks { get; set; } = new List<SalesmanStock>();
}
