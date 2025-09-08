using Microsoft.AspNetCore.Mvc;
using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Common.Services;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;

namespace ReportHub.Controllers
{
    /// <summary>
    /// Legacy Reports Controller - Maintains backward compatibility
    /// For new functionality, use TemplateReportsController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportGeneratorService _reportGenerator;
        private readonly ITemplateEngineService _templateEngineService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            IReportGeneratorService reportGenerator,
            ITemplateEngineService templateEngineService,
            ILogger<ReportsController> logger)
        {
            _reportGenerator = reportGenerator;
            _templateEngineService = templateEngineService;
            _logger = logger;
        }

        /// <summary>
        /// Generate report with format selection
        /// Now supports both legacy and template-based generation
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Use template-based generator which supports backward compatibility
            var result = await _reportGenerator.GenerateReportAsync(request.Settings, request.Data, request.Format);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return File(result.Data, result.ContentType, result.FileName);
        }

        /// <summary>
        /// Generate PDF report using template-based system
        /// Maintains backward compatibility with existing API
        /// </summary>
        [HttpPost("generate-pdf")]
        public async Task<IActionResult> GeneratePdfReport([FromBody] ReportRequestBase request)
        {
            try
            {
                _logger.LogInformation("Generating PDF report using template engine (legacy endpoint)");
                
                // Use the template-based generator which now handles legacy format conversion
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
            var result = await _reportGenerator.GenerateReportAsync<Word_GeneratorHelper>(request.Settings, request.Data);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return File(result.Data, result.ContentType, result.FileName);
        }

        [HttpPost("generate-excel")]
        public async Task<IActionResult> GenerateExcelReport([FromBody] ReportRequestBase request)
        {
            var result = await _reportGenerator.GenerateReportAsync<Excel_GeneratorHelper>(request.Settings, request.Data);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return File(result.Data, result.ContentType, result.FileName);
        }

        /// <summary>
        /// Get supported formats including new template-based formats
        /// </summary>
        [HttpGet("supported-formats")]
        public IActionResult GetSupportedFormats()
        {
            var formats = Enum.GetValues<ReportFormat>()
                .Where(format => _reportGenerator.SupportsFormat(format))
                .Select(format => new
                {
                    Format = format.ToString(),
                    ContentType = _reportGenerator.GetContentType(format)
                });

            return Ok(formats);
        }

        /// <summary>
        /// Get available templates (new functionality)
        /// </summary>
        [HttpGet("templates")]
        public IActionResult GetAvailableTemplates()
        {
            try
            {
                var templates = _templateEngineService.GetAvailableTemplates();
                return Ok(new 
                { 
                    message = "This endpoint provides template information. For full template functionality, use /api/templatereports/templates",
                    templates = templates.Select(t => new 
                    {
                        t.TemplateId,
                        t.DisplayName,
                        t.Description,
                        t.Category
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving templates through legacy endpoint");
                return StatusCode(500, new { error = "An error occurred while retrieving templates" });
            }
        }
    }

    // Request models (maintained for backward compatibility)
    public class ReportRequest : ReportRequestBase
    {
        public ReportFormat Format { get; set; }
    }

    public class ReportRequestBase
    {
        public ReportSettings Settings { get; set; } = new();
        public ReportData Data { get; set; } = new();
    }
}