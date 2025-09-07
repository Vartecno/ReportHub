using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
