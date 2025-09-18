namespace ReportHub.Objects.DTOs
{
    public class StatementHeaderDTO
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, string> CustomFields { get; set; } = new();
    }    
}