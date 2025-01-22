using System;
using System.Collections.Generic;

namespace StockManagement.Models;



using StockManagement.Models;

public class ItemSupplier
{
    public int ItemSupplierId { get; set; }
    public int ItemId { get; set; }
    public int SupplierId { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }

    public virtual Item? Item { get; set; }
    public virtual Supplier? Supplier { get; set; }
}
