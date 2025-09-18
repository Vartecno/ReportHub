namespace ReportHub.Objects.DTOs
{
    public class AccountStatementDataDTO
    {
        public StatementHeaderDTO Header { get; set; } = new();
        public List<TransactionDTO> Transactions { get; set; } = new();
        public StatementSummaryDTO Summary { get; set; } = new();
        public Dictionary<string, object> AdditionalFields { get; set; } = new();
    }
}