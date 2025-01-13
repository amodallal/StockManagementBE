using System;
using System.Collections.Generic;

namespace StockManagement.Models;


public class Capacity
{
    public int CapacityID { get; set; } // Primary Key
    public string CapacityName { get; set; } // Name of the capacity

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}