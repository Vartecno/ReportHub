using System.ComponentModel.DataAnnotations;
using ReportHub.Objects.DTOs.Common;
using ReportHub.Objects.HelperModel;

namespace ReportHub.Objects.DTOs
{
    
    public class TemplateReportRequestDTO
    {        
        [Required]
        public string ReportType { get; set; } = string.Empty;        
        [Required]
        public ReportBrandingDTO Branding { get; set; } = new();        
        [Required]
        public object Data { get; set; } = new();        
        public ReportConfigurationDTO Configuration { get; set; } = new();        
        public Dictionary<string, object> StylingOverrides { get; set; } = new();        
        public Dictionary<string, object> Metadata { get; set; } = new();
    }    
}