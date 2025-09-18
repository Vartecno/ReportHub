using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Objects.DTOs
{
    public class SalesInvoiceDataDTO
    {
        public InvoiceHeaderDTO Header { get; set; } = new();
        public ShipmentDetailsDTO Shipment { get; set; } = new();
        public List<ChargeItemDTO> Charges { get; set; } = new();
        public InvoiceTotalDTO Total { get; set; } = new();
        public List<BankAccountDTO> BankAccounts { get; set; } = new();
        public Dictionary<string, object> AdditionalFields { get; set; } = new();
    }
}
