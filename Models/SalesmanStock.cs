using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class SalesmanStock
{

    public int SalesmanStockId { get; set; }
   
    public int EmployeeId { get; set; }

    public int ItemId { get; set; }

    public int? Quantity { get; set; }
    public string? SerialNumber { get; set; }

    public string? Barcode { get; set; }
    public string? Imei1 { get; set; }

    public string? Imei2 { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? Cost { get; set; }

    public DateOnly DateReceived { get; set; }

    public DateOnly TransferDate { get; set; }

    public int? StatusId { get; set; }


    public virtual Item Item { get; set; } = null!;


    public virtual Status Status { get; set; } = null!;

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();


}
