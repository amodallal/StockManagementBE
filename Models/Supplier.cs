using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<ItemDetail> ItemDetails { get; set; } = new List<ItemDetail>();

    public virtual ICollection<ItemSupplier> ItemSuppliers { get; set; } = new List<ItemSupplier>();
}
