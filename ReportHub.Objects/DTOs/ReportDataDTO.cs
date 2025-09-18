using ReportHub.Objects.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Objects.DTOs
{

    public class OrderLeadDetailsDTO
    {
        public int OrderLeadID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string InvoiceID { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Volume { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }
    public class OrderProfitResultDTO
    {
        public string OrderLeadCode { get; set; } = string.Empty;
        public string InvoiceID { get; set; } = string.Empty;
        public decimal TotalSalesPrice { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Volume { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = "JOD";
    }
}
