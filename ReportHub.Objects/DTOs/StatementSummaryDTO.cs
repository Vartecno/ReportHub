namespace ReportHub.Objects.DTOs
{
    public class StatementSummaryDTO
    {
        public decimal OpeningBalance { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal ClosingBalance { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, decimal> AdditionalTotals { get; set; } = new();
    }
}