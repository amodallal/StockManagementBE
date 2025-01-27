using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Status
{
    public int status_id { get; set; }

    public string StatusName{ get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<SalesmanStock> SalesmanStocks { get; set; }


    public virtual ICollection<Delivery> Deliveries { get; set; }

}
