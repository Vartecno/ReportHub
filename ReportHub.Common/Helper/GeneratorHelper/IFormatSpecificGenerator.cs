using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;

namespace ReportHub.Common.Helper.GeneratorHelper
{
    public interface IFormatSpecificGenerator
    {
        ReportFormat SupportedFormat { get; }
        string ContentType { get; }
        Task<byte[]> GenerateAsync(ReportSettings settings, ReportData data);
        Task<Stream> GenerateStreamAsync(ReportSettings settings, ReportData data);
    }
}
