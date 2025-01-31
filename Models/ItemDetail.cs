using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class ItemDetail
{
    public int ItemDetailsId { get; set; }

    public int? ItemId { get; set; }

    public string? SerialNumber { get; set; }

    public string? Imei1 { get; set; }

    public string? Imei2 { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? Cost { get; set; }

    public DateOnly? DateReceived { get; set; }

    public string? Barcode { get; set; }
    public int? SupplierId { get; set; }

    public int? Quantity { get; set; }


    public virtual Item? Item { get; set; } = null!;

    public virtual Supplier? Supplier { get; set; }
}
