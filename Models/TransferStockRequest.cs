public class TransferStockRequest
{
    public int Employee_id { get; set; }
    public int ItemId { get; set; }
    public int TransferQuantity { get; set; }

    public string Source { get; set; }

    public string Destination { get; set; }
}
