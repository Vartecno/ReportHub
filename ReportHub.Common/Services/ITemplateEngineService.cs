using ReportHub.Objects.DTOs;
using ReportHub.Objects.HelperModel;
using ReportHub.Objects.Interfaces;

namespace ReportHub.Common.Services
{
    /// <summary>
    /// Service interface for template engine operations
    /// </summary>
    public interface ITemplateEngineService
    {
        /// <summary>
        /// Generate a report using template engine
        /// </summary>
        Task<ReportResult> GenerateTemplateReportAsync(TemplateReportRequestDTO request);

        /// <summary>
        /// Get available templates
        /// </summary>
        List<TemplateMetadata> GetAvailableTemplates();

        /// <summary>
        /// Validate a template request
        /// </summary>
        TemplateValidationResult ValidateTemplateRequest(TemplateReportRequestDTO request);

        /// <summary>
        /// Get sample data for a specific template
        /// </summary>
        object? GetTemplateSampleData(string templateId);

        /// <summary>
        /// Get configuration schema for a specific template
        /// </summary>
        TemplateConfigSchema? GetTemplateConfigSchema(string templateId);
    }

    /// <summary>
    /// Template engine service implementation
    /// </summary>
    public class TemplateEngineService : ITemplateEngineService
    {
        private readonly ITemplateEngine _templateEngine;

        public TemplateEngineService(ITemplateEngine templateEngine)
        {
            _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        }

        public Task<ReportResult> GenerateTemplateReportAsync(TemplateReportRequestDTO request)
        {
            return _templateEngine.GenerateReportAsync(request);
        }

        public List<TemplateMetadata> GetAvailableTemplates()
        {
            return _templateEngine.GetAvailableTemplates();
        }

        public TemplateValidationResult ValidateTemplateRequest(TemplateReportRequestDTO request)
        {
            return _templateEngine.ValidateRequest(request);
        }

        public object? GetTemplateSampleData(string templateId)
        {
            var template = _templateEngine.GetTemplate(templateId);
            return template?.GetSampleData();
        }

        public TemplateConfigSchema? GetTemplateConfigSchema(string templateId)
        {
            var template = _templateEngine.GetTemplate(templateId);
            return template?.GetConfigSchema();
        }
    }
}