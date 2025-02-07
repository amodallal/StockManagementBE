using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagement.Models;

public partial class TransfersHistory
{
    
    public int TransferHistoryId { get; set; }

    
    public int ItemId { get; set; }

   
    public string IMEI1 { get; set; } = null!;

    
    public string IMEI2 { get; set; } = null!;

    
    public string SerialNumber { get; set; } = null!;

    public DateTime DateTransfered { get; set; }

   
    public string Source { get; set; } = null!;

    
    public string Destination { get; set; } = null!;


    public int quantity { get; set; }

    
    public string? Note { get; set; }

    public virtual Item Item { get; set; } = null!;
}
