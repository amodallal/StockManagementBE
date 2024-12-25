using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? EmployeeId { get; set; }

    public int? CustomerId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? StatusId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

    public virtual Status? Status { get; set; }
}
