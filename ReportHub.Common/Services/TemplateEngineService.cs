using ReportHub.Objects.DTOs;
using ReportHub.Objects.HelperModel;
using ReportHub.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Common.Services
{
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

        public object GetTemplateSampleData(string templateId)
        {
            var template = _templateEngine.GetTemplate(templateId);
            return template?.GetSampleData();
        }

        public TemplateConfigSchema GetTemplateConfigSchema(string templateId)
        {
            var template = _templateEngine.GetTemplate(templateId);
            return template?.GetConfigSchema();
        }
    }
}
