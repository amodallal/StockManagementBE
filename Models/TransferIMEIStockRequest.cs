﻿namespace StockManagement.Models
{
    public class TransferIMEIStockRequest
    {
        public int Employee_id { get; set; }
        public string IMEI_1 { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

    }
}
