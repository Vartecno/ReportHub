namespace ReportHub.Objects.DTOs
{
    public class ShipmentDetailsDTO
    {
        public string Service { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string AWB { get; set; } = string.Empty;
        public string MBL { get; set; } = string.Empty;
        public string HBL { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Volume { get; set; } = string.Empty;
        public Dictionary<string, object> CustomFields { get; set; } = new();
    }    
}