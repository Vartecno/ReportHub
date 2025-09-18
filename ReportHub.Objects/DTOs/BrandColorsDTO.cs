namespace ReportHub.Objects.DTOs
{
    public class BrandColorsDTO
    {
        public string Primary { get; set; } = "#0066CC";
        public string Secondary { get; set; } = "#F0F0F0";
        public string Accent { get; set; } = "#FF6600";
        public string Text { get; set; } = "#333333";
        public string Background { get; set; } = "#FFFFFF";
        public Dictionary<string, string> Custom { get; set; } = new();
    }
}