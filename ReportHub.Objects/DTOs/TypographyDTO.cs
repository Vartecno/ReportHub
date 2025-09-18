namespace ReportHub.Objects.DTOs
{
    public class TypographyDTO
    {
        public string FontFamily { get; set; } = "Arial";
        public int HeaderSize { get; set; } = 18;
        public int BodySize { get; set; } = 11;
        public int SmallSize { get; set; } = 9;
        public Dictionary<string, object> CustomFonts { get; set; } = new();
    }
}