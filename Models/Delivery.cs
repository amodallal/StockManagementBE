using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Delivery
{
    public int DeliveryId { get; set; }

    public int OrderId { get; set; }

    public int EmployeeId { get; set; }

    public decimal CashCollected { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public string? DeliveryStatus { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
