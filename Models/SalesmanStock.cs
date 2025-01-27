using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class SalesmanStock
{
    public int ItemDetailsId { get; set; }

    public int EmployeeId { get; set; }

    public int ItemId { get; set; }

    public int DescriptionId { get; set; }

    public string? SerialNumber { get; set; }

    public string? Imei1 { get; set; }

    public string? Imei2 { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? Cost { get; set; }

    public DateOnly DateReceived { get; set; }

    public int? SupplierId { get; set; }

    public int? StatusId { get; set; }


    public virtual Item Item { get; set; } = null!;

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

}
