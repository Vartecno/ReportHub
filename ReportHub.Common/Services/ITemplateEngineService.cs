using ReportHub.Objects.DTOs;
using ReportHub.Objects.HelperModel;
using ReportHub.Objects.Interfaces;

namespace ReportHub.Common.Services
{
    
    public interface ITemplateEngineService
    {        
        Task<ReportResult> GenerateTemplateReportAsync(TemplateReportRequestDTO request);        
        List<TemplateMetadata> GetAvailableTemplates();        
        TemplateValidationResult ValidateTemplateRequest(TemplateReportRequestDTO request);        
        object? GetTemplateSampleData(string templateId);        
        TemplateConfigSchema? GetTemplateConfigSchema(string templateId);
    }

    

}