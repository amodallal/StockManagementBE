namespace StockManagement.Models
{
    public class OrderedItemDto
    {
        public int OrderedItemId { get; set; }
        public int ItemDetailsId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ItemName { get; set; }
        public string ModelNumber { get; set; }
        public string Barcode { get; set; }
        public string SerialNumber { get; set; }
        public string Imei1 { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
