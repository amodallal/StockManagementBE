namespace StockManagement.Models
{
    public class TransferSerialStockRequest
    {
        public int Employee_id { get; set; }
        public string SerialNumber { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }
    }
}
