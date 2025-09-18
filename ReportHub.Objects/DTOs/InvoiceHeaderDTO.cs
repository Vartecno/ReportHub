namespace ReportHub.Objects.DTOs
{
    public class InvoiceHeaderDTO
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public Dictionary<string, string> CustomFields { get; set; } = new();
    }    
}