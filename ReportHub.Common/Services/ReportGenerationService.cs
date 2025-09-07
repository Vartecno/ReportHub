using Microsoft.Extensions.Logging;
using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Common.Services;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Common.Services
{
    public class ReportGenerationService : IReportGeneratorService
    {
        private readonly Dictionary<ReportFormat, IFormatSpecificGenerator> _generators;
        private readonly ILogger<ReportGenerationService>? _logger;

        public ReportGenerationService(IEnumerable<IFormatSpecificGenerator> generators, ILogger<ReportGenerationService>? logger = null)
        {
            _generators = generators.ToDictionary(g => g.SupportedFormat, g => g);
            _logger = logger;
        }

        public async Task<ReportResult> GenerateReportAsync(ReportSettings settings, ReportData data, ReportFormat format)
        {
            try
            {
                // Validate inputs
                var validationResult = ValidateInputs(settings, data);
                if (!validationResult.IsValid)
                {
                    return new ReportResult
                    {
                        Success = false,
                        Errors = validationResult.Errors
                    };
                }

                // Check if format is supported
                if (!_generators.TryGetValue(format, out var generator))
                {
                    return new ReportResult
                    {
                        Success = false,
                        Errors = new List<string> { $"Report format '{format}' is not supported" }
                    };
                }

                _logger?.LogInformation("Generating {Format} report: {Title}", format, settings.Title);

                // Generate report
                var reportData = await generator.GenerateAsync(settings, data);
                var fileName = GenerateFileName(settings.Title, format);

                return new ReportResult
                {
                    Data = reportData,
                    ContentType = generator.ContentType,
                    FileName = fileName,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error generating {Format} report: {Title}", format, settings.Title);

                return new ReportResult
                {
                    Success = false,
                    Errors = new List<string> { $"An error occurred while generating the report: {ex.Message}" }
                };
            }
        }

        public async Task<ReportResult> GenerateReportAsync<T>(ReportSettings settings, ReportData data) where T : IFormatSpecificGenerator
        {
            var generator = _generators.Values.FirstOrDefault(g => g is T);
            if (generator == null)
            {
                return new ReportResult
                {
                    Success = false,
                    Errors = new List<string> { $"Generator of type '{typeof(T).Name}' is not registered" }
                };
            }

            return await GenerateReportAsync(settings, data, generator.SupportedFormat);
        }

        public bool SupportsFormat(ReportFormat format)
        {
            return _generators.ContainsKey(format);
        }

        public string GetContentType(ReportFormat format)
        {
            return _generators.TryGetValue(format, out var generator) ? generator.ContentType : string.Empty;
        }


        private ValidationResult ValidateInputs(ReportSettings settings, ReportData data)
        {
            var errors = new List<string>();

            // Validate settings
            var context = new ValidationContext(settings);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!Validator.TryValidateObject(settings, context, validationResults, true))
            {
                errors.AddRange(validationResults.Select(vr => vr.ErrorMessage ?? "Validation error"));
            }

            // Additional business rules validation
            if (string.IsNullOrWhiteSpace(settings.Title))
            {
                errors.Add("Report title is required");
            }

            if (data == null)
            {
                errors.Add("Report data cannot be null");
            }
            else if (!data.Sections.Any() && !data.Tables.Any() && !data.Charts.Any())
            {
                errors.Add("Report must contain at least one section, table, or chart");
            }

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        private string GenerateFileName(string title, ReportFormat format)
        {
            var safeTitle = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var extension = format switch
            {
                ReportFormat.PDF => "pdf",
                ReportFormat.Word => "docx",
                ReportFormat.Excel => "xlsx",
                _ => "bin"
            };

            return $"{safeTitle}_{timestamp}.{extension}";
        }

        private class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new();
        }
    }
}
