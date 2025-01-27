﻿using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Status
{
    public int Id { get; set; }

    public string Status1 { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

   
}
