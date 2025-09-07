using ReportHub.Common.Helper.GeneratorHelper;
using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;

namespace ReportHub.Common.Services
{
    public interface IReportGeneratorService
    {
        Task<ReportResult> GenerateReportAsync(ReportSettings settings, ReportData data, ReportFormat format);
        Task<ReportResult> GenerateReportAsync<T>(ReportSettings settings, ReportData data) where T : IFormatSpecificGenerator;
        string GetContentType(ReportFormat format);
        bool SupportsFormat(ReportFormat format);
    }
}