namespace ReportHub.Objects.DTOs
{
    public class InvoiceTotalDTO
    {
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, decimal> CurrencyTotals { get; set; } = new();
    }    
}