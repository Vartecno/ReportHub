using System.ComponentModel.DataAnnotations;

namespace ReportHub.Objects.DTOs
{
    public class ReportBrandingDTO
    {        
        [Required]
        public CompanyInfoDTO Company { get; set; } = new();        
        public string? Logo { get; set; }        
        public Dictionary<string, string> Icons { get; set; } = new();        
        public BrandColorsDTO Colors { get; set; } = new();        
        public TypographyDTO Typography { get; set; } = new();
    }
}