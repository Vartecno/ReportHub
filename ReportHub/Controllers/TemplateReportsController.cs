using Microsoft.AspNetCore.Mvc;
using ReportHub.Common.Services;
using ReportHub.Objects.DTOs;
using ReportHub.Objects.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ReportHub.Controllers
{
    /// <summary>
    /// Controller for template-based report generation
    /// Supports 50+ report types with dynamic branding and logos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateReportsController : ControllerBase
    {
        private readonly ITemplateEngineService _templateEngineService;
        private readonly ILogger<TemplateReportsController> _logger;

        public TemplateReportsController(
            ITemplateEngineService templateEngineService,
            ILogger<TemplateReportsController> logger)
        {
            _templateEngineService = templateEngineService ?? throw new ArgumentNullException(nameof(templateEngineService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Generate a report using the template engine
        /// Supports dynamic logos, branding, and 50+ report types
        /// </summary>
        /// <param name="request">Template report request with data and branding</param>
        /// <returns>Generated PDF report</returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTemplateReport([FromBody] TemplateReportRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Generating template report: {ReportType}", request.ReportType);

                // Validate request
                var validation = _templateEngineService.ValidateTemplateRequest(request);
                if (!validation.IsValid)
                {
                    return BadRequest(new { errors = validation.Errors, warnings = validation.Warnings });
                }

                // Generate report
                var result = await _templateEngineService.GenerateTemplateReportAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                _logger.LogInformation("Template report generated successfully: {FileName} ({Size} bytes)",
                    result.FileName, result.FileSizeBytes);

                return File(result.Data, result.ContentType, result.FileName);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request data for template: {ReportType}", request.ReportType);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating template report: {ReportType}", request.ReportType);
                return StatusCode(500, new { error = "An internal error occurred while generating the report" });
            }
        }

        /// <summary>
        /// Get all available report templates
        /// </summary>
        /// <returns>List of available templates with metadata</returns>
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

        /// <summary>
        /// Get sample data for a specific template
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Sample data structure for the template</returns>
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

        /// <summary>
        /// Get configuration schema for a specific template
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>Configuration schema for the template</returns>
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

        /// <summary>
        /// Validate a template request without generating the report
        /// </summary>
        /// <param name="request">Template report request to validate</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate")]
        public IActionResult ValidateTemplateRequest([FromBody] TemplateReportRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var validation = _templateEngineService.ValidateTemplateRequest(request);
                return Ok(validation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating template request: {ReportType}", request.ReportType);
                return StatusCode(500, new { error = "An error occurred while validating the request" });
            }
        }

        /// <summary>
        /// Generate a venues invoice (backward compatibility endpoint)
        /// </summary>
        /// <param name="request">Venues invoice data</param>
        /// <returns>Generated PDF invoice</returns>
        [HttpPost("venues-invoice")]
        public async Task<IActionResult> GenerateVenuesInvoice([FromBody] VenuesInvoiceRequestDTO request)
        {
            try
            {
                var templateRequest = new TemplateReportRequestDTO
                {
                    ReportType = "venues_invoice",
                    Branding = request.Branding,
                    Data = request.Data,
                    Configuration = request.Configuration ?? new ReportConfigurationDTO { Title = "Invoice" }
                };

                return await GenerateTemplateReport(templateRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating venues invoice");
                return StatusCode(500, new { error = "An error occurred while generating the invoice" });
            }
        }

        /// <summary>
        /// Generate an account statement (backward compatibility endpoint)
        /// </summary>
        /// <param name="request">Account statement data</param>
        /// <returns>Generated PDF statement</returns>
        [HttpPost("account-statement")]
        public async Task<IActionResult> GenerateAccountStatement([FromBody] AccountStatementRequestDTO request)
        {
            try
            {
                var templateRequest = new TemplateReportRequestDTO
                {
                    ReportType = "account_statement",
                    Branding = request.Branding,
                    Data = request.Data,
                    Configuration = request.Configuration ?? new ReportConfigurationDTO { Title = "Account Statement" }
                };

                return await GenerateTemplateReport(templateRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating account statement");
                return StatusCode(500, new { error = "An error occurred while generating the statement" });
            }
        }
    }

    #region Backward Compatibility DTOs

    /// <summary>
    /// Backward compatibility DTO for venues invoice
    /// </summary>
    public class VenuesInvoiceRequestDTO
    {
        [Required]
        public ReportBrandingDTO Branding { get; set; } = new();
        
        [Required]
        public VenuesInvoiceDataDTO Data { get; set; } = new();
        
        public ReportConfigurationDTO? Configuration { get; set; }
    }

    /// <summary>
    /// Backward compatibility DTO for account statement
    /// </summary>
    public class AccountStatementRequestDTO
    {
        [Required]
        public ReportBrandingDTO Branding { get; set; } = new();
        
        [Required]
        public AccountStatementDataDTO Data { get; set; } = new();
        
        public ReportConfigurationDTO? Configuration { get; set; }
    }

    #endregion
}