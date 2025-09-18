namespace ReportHub.Objects.DTOs
{
    public class ChargeItemDTO
    {
        public int Sequence { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal RatePerUnit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; } = "JOD";
        public Dictionary<string, object> CustomFields { get; set; } = new();
    }    
}