using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Description
{
    public int DescriptionId { get; set; }

    public string DescriptionText { get; set; } = null!;

    public virtual ICollection<ItemDetail> ItemDetails { get; set; } = new List<ItemDetail>();
}
