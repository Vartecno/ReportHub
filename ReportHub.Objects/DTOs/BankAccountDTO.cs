namespace ReportHub.Objects.DTOs
{
    public class BankAccountDTO
    {
        public string BankName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string SWIFT { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalInfo { get; set; } = new();
    }    
}