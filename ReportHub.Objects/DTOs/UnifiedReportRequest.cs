using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Objects.DTOs
{
    /// <summary>
    /// Unified request model that supports both template-based and legacy generation
    /// </summary>
    public class UnifiedReportRequest
    {        
        [Required]
        public ReportFormat Format { get; set; } = ReportFormat.PDF;        
        public string? ReportType { get; set; }        
        public ReportBrandingDTO? Branding { get; set; }        
        public object? Data { get; set; }        
        public ReportConfigurationDTO? Configuration { get; set; }        
        public ReportSettings? Settings { get; set; }        
        public ReportData? LegacyData { get; set; }
    }
}
