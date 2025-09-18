using Microsoft.AspNetCore.Mvc;
using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Common.Services;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;
using ReportHub.Objects.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ReportHub.Controllers
{
    /// <summary>
    /// Unified Report Controller - Handles all report generation with format selection
    /// Supports PDF, Word, and Excel formats through a single API endpoint
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportGeneratorService _reportGenerator;
        private readonly ITemplateEngineService _templateEngineService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(
            IReportGeneratorService reportGenerator,
            ITemplateEngineService templateEngineService,
            ILogger<ReportController> logger)
        {
            _reportGenerator = reportGenerator;
            _templateEngineService = templateEngineService;
            _logger = logger;
        }
        
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromBody] UnifiedReportRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Generating {Format} report using unified endpoint", request.Format);

                 if (!string.IsNullOrEmpty(request.ReportType))
                {
                    var templateRequest = new TemplateReportRequestDTO
                    {
                        ReportType = request.ReportType,
                        Branding = request.Branding ?? new ReportBrandingDTO(),
                        Data = request.Data ?? new(),
                        Configuration = request.Configuration ?? new ReportConfigurationDTO { Title = "Report" }
                    };

                     var validation = _templateEngineService.ValidateTemplateRequest(templateRequest);
                    if (!validation.IsValid)
                    {
                        return BadRequest(new { errors = validation.Errors, warnings = validation.Warnings });
                    }

                     var templateResult = await _templateEngineService.GenerateTemplateReportAsync(templateRequest);
                    
                    if (!templateResult.Success)
                    {
                        return BadRequest(new { errors = templateResult.Errors });
                    }

                     if (request.Format != ReportFormat.PDF)
                    {
                        var convertedResult = await ConvertReportFormat(templateResult, request.Format);
                        if (!convertedResult.Success)
                        {
                            return BadRequest(new { errors = convertedResult.Errors });
                        }
                        return File(convertedResult.Data, convertedResult.ContentType, convertedResult.FileName);
                    }

                    return File(templateResult.Data, templateResult.ContentType, templateResult.FileName);
                }
                else
                {
                     var legacySettings = request.Settings ?? new ReportSettings();
                    var legacyData = request.LegacyData ?? new ReportData();
                    
                    var result = await _reportGenerator.GenerateReportAsync(legacySettings, legacyData, request.Format);

                    if (!result.Success)
                    {
                        return BadRequest(new { errors = result.Errors });
                    }

                    return File(result.Data, result.ContentType, result.FileName);
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request data for report generation");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating {Format} report", request.Format);
                return StatusCode(500, new { error = "An internal error occurred while generating the report" });
            }
        }
        
        [HttpPost("generate-pdf")]
        public async Task<IActionResult> GeneratePdfReport([FromBody] ReportRequestBase request)
        {
            try
            {
                _logger.LogInformation("Generating PDF report using legacy endpoint");
                
                var result = await _reportGenerator.GenerateReportAsync<TemplateBasedPDFGenerator>(request.Settings, request.Data);

                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return File(result.Data, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF report through legacy endpoint");
                return StatusCode(500, new { error = "An error occurred while generating the report" });
            }
        }
        
        [HttpPost("generate-word")]
        public async Task<IActionResult> GenerateWordReport([FromBody] ReportRequestBase request)
        {
            try
            {
                var result = await _reportGenerator.GenerateReportAsync<Word_GeneratorHelper>(request.Settings, request.Data);

                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return File(result.Data, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Word report");
                return StatusCode(500, new { error = "An error occurred while generating the Word report" });
            }
        }
        
        [HttpPost("generate-excel")]
        public async Task<IActionResult> GenerateExcelReport([FromBody] ReportRequestBase request)
        {
            try
            {
                var result = await _reportGenerator.GenerateReportAsync<Excel_GeneratorHelper>(request.Settings, request.Data);

                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return File(result.Data, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel report");
                return StatusCode(500, new { error = "An error occurred while generating the Excel report" });
            }
        }
        
        [HttpPost("sales-invoice")]
        public async Task<IActionResult> GenerateSalesInvoice([FromBody] SalesInvoiceRequest request)
        {
            try
            {
                var unifiedRequest = new UnifiedReportRequest
                {
                    ReportType = "sales_invoice",
                    Format = request.Format,
                    Branding = request.Branding,
                    Data = request.Data,
                    Configuration = request.Configuration ?? new ReportConfigurationDTO { Title = "Sales Invoice" }
                };

                return await GenerateReport(unifiedRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sales invoice");
                return StatusCode(500, new { error = "An error occurred while generating the invoice" });
            }
        }
        
        [HttpPost("account-statement")]
        public async Task<IActionResult> GenerateAccountStatement([FromBody] AccountStatementRequest request)
        {
            try
            {
                var unifiedRequest = new UnifiedReportRequest
                {
                    ReportType = "account_statement",
                    Format = request.Format,
                    Branding = request.Branding,
                    Data = request.Data,
                    Configuration = request.Configuration ?? new ReportConfigurationDTO { Title = "Account Statement" }
                };

                return await GenerateReport(unifiedRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating account statement");
                return StatusCode(500, new { error = "An error occurred while generating the statement" });
            }
        }
        
        [HttpGet("supported-formats")]
        public IActionResult GetSupportedFormats()
        {
            var formats = Enum.GetValues<ReportFormat>()
                .Where(format => _reportGenerator.SupportsFormat(format))
                .Select(format => new
                {
                    Format = format.ToString(),
                    ContentType = _reportGenerator.GetContentType(format),
                    Description = GetFormatDescription(format)
                });

            return Ok(new { formats, message = "Supported output formats for report generation" });
        }
        
        [HttpGet("templates")]
        public IActionResult GetAvailableTemplates()
        {
            try
            {
                var templates = _templateEngineService.GetAvailableTemplates();
                return Ok(new { templates, count = templates.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available templates");
                return StatusCode(500, new { error = "An error occurred while retrieving templates" });
            }
        }
        
        [HttpGet("templates/{templateId}/sample")]
        public IActionResult GetTemplateSampleData([Required] string templateId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(templateId))
                {
                    return BadRequest(new { error = "Template ID is required" });
                }

                var sampleData = _templateEngineService.GetTemplateSampleData(templateId);
                if (sampleData == null)
                {
                    return NotFound(new { error = $"Template '{templateId}' not found" });
                }

                return Ok(new { templateId, sampleData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sample data for template: {TemplateId}", templateId);
                return StatusCode(500, new { error = "An error occurred while retrieving sample data" });
            }
        }
        
        [HttpGet("templates/{templateId}/schema")]
        public IActionResult GetTemplateConfigSchema([Required] string templateId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(templateId))
                {
                    return BadRequest(new { error = "Template ID is required" });
                }

                var schema = _templateEngineService.GetTemplateConfigSchema(templateId);
                if (schema == null)
                {
                    return NotFound(new { error = $"Template '{templateId}' not found" });
                }

                return Ok(schema);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving config schema for template: {TemplateId}", templateId);
                return StatusCode(500, new { error = "An error occurred while retrieving config schema" });
            }
        }
        
        [HttpPost("validate")]
        public IActionResult ValidateReportRequest([FromBody] UnifiedReportRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!string.IsNullOrEmpty(request.ReportType))
                {
                    var templateRequest = new TemplateReportRequestDTO
                    {
                        ReportType = request.ReportType,
                        Branding = request.Branding ?? new ReportBrandingDTO(),
                        Data = request.Data ?? new(),
                        Configuration = request.Configuration ?? new ReportConfigurationDTO()
                    };

                    var validation = _templateEngineService.ValidateTemplateRequest(templateRequest);
                    return Ok(validation);
                }

                return Ok(new { IsValid = true, Errors = new List<string>(), Warnings = new List<string>() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating report request");
                return StatusCode(500, new { error = "An error occurred while validating the request" });
            }
        }

        #region Private Methods

        private async Task<ReportResult> ConvertReportFormat(ReportResult pdfResult, ReportFormat targetFormat)
        {
            // This would be implemented to convert PDF to Word/Excel
            // For now, return the original result as this would require additional conversion logic
            _logger.LogWarning("Format conversion from PDF to {TargetFormat} not yet implemented", targetFormat);
            return pdfResult;
        }

        private string GetFormatDescription(ReportFormat format)
        {
            return format switch
            {
                ReportFormat.PDF => "Portable Document Format - best for printing and sharing",
                ReportFormat.Word => "Microsoft Word document - editable format",
                ReportFormat.Excel => "Microsoft Excel spreadsheet - best for data analysis",
                _ => "Unknown format"
            };
        }

        #endregion
    }

    
}
