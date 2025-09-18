namespace ReportHub.Objects.DTOs
{
    public class TransactionDTO
    {
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, object> AdditionalFields { get; set; } = new();
    }    
}