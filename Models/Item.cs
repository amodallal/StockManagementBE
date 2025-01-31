using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    
    public string ModelNumber { get; set; }

    public int? BrandId { get; set; }

    public int CategoryId { get; set; }


    public bool IsImeiId { get; set; }

    public int? ColorId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; } = null!;


    public string Description { get; set; }

    public virtual Color? Color { get; set; }

    public virtual ICollection<ItemDetail> ItemDetails { get; set; } = new List<ItemDetail>();

    public virtual ICollection<SalesmanStock> SalesmanStocks { get; set; } = new List<SalesmanStock>();

    public virtual ICollection<Capacity> Capacities { get; set; } = new List<Capacity>();


    public virtual ICollection<ItemSupplier> ItemSuppliers { get; set; } = new List<ItemSupplier>();
}
