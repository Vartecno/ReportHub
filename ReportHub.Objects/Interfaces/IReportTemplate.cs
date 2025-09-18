using QuestPDF.Infrastructure;
using ReportHub.Objects.DTOs;

namespace ReportHub.Objects.Interfaces
{
    
    public interface IReportTemplate
    {        
        string TemplateId { get; }        
        string DisplayName { get; }        
        string Description { get; }        
        string Version { get; }        
        Type[] SupportedDataTypes { get; }        
        void GenerateContent(IContainer container, TemplateReportRequestDTO request);        
        TemplateValidationResult ValidateRequest(TemplateReportRequestDTO request);        
        object GetSampleData();        
        TemplateConfigSchema GetConfigSchema();
    }
}