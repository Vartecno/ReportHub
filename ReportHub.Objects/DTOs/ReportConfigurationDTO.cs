namespace ReportHub.Objects.DTOs
{
    public class ReportConfigurationDTO
    {       
        public string Title { get; set; } = string.Empty;        
        public string Subtitle { get; set; } = string.Empty;        
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;        
        public string Author { get; set; } = "System Generated";        
        public bool IncludePageNumbers { get; set; } = true;        
        public bool IncludeHeader { get; set; } = true;
        public bool IncludeFooter { get; set; } = true;        
        public string Culture { get; set; } = "en-US";
        public bool IsRightToLeft { get; set; } = false;        
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }    
}