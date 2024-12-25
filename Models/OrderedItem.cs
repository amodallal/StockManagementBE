using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class OrderedItem
{
    public int OrderedItemId { get; set; }

    public int OrderId { get; set; }

    public int ItemDetailsId { get; set; }

    public int Quantity { get; set; }

    public decimal? Discount { get; set; }

    public decimal? Amount { get; set; }

    public virtual SalesmanStock ItemDetails { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
