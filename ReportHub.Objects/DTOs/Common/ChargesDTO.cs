using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Objects.DTOs.Common
{
    public class ChargesDTO
    {
        public string Description { get; set; }
        public string RatePER { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        public string CurrencyName { get; set; }
        public int CurrencyID { get; set; }
        public decimal ExchangeRate { get; set; }
        public string InvoiceID { get; set; }
        public int TransType { get; set; }

        public decimal Discount { get; set; }
    }
}
