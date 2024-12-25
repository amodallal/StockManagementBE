using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class StockTransfer
{
    public int TransferId { get; set; }

    public int ItemDetailsId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime DateTransfered { get; set; }

    public DateTime? DateReceived { get; set; }

    public int? StatusId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<SalesmanStock> SalesmanStocks { get; set; } = new List<SalesmanStock>();

    public virtual Status? Status { get; set; }
}
