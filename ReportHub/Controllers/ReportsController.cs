using Microsoft.AspNetCore.Mvc;
using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Common.Services;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;

namespace ReportHub.Controllers
{
    public class ReportsController : ControllerBase
    {
        private readonly IReportGeneratorService _reportGenerator;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportGeneratorService reportGenerator, ILogger<ReportsController> logger)
        {
            _reportGenerator = reportGenerator;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _reportGenerator.GenerateReportAsync(request.Settings, request.Data, request.Format);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return File(result.Data, result.ContentType, result.FileName);
        }

        [HttpPost("generate-pdf")]
        public async Task<IActionResult> GeneratePdfReport([FromBody] ReportRequestBase request)
        {
            var result = await _reportGenerator.GenerateReportAsync<PDF_GeneratorHelper>(request.Settings, request.Data);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return File(result.Data, result.ContentType, result.FileName);
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
    }

    // Request models
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
