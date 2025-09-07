using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportHub.Objects.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace ReportHub.Objects.HelperModel
{
    public class ReportSettings
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Subtitle { get; set; }

        [Required]
        public string Author { get; set; } = "System Generated";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? Description { get; set; }

        public Dictionary<string, string> Metadata { get; set; } = new();

        public List<string> Headers { get; set; } = new();

        public ReportFormattingOptions Formatting { get; set; } = new();

        public string Culture { get; set; } = CultureInfo.CurrentCulture.Name;
    }

    // Formatting options
    public class ReportFormattingOptions
    {
        public string FontFamily { get; set; } = "Arial";
        public int FontSize { get; set; } = 12;
        public bool IncludePageNumbers { get; set; } = true;
        public bool IncludeHeader { get; set; } = true;
        public bool IncludeFooter { get; set; } = true;
        public string PrimaryColor { get; set; } = "#0066CC";
        public string SecondaryColor { get; set; } = "#F0F0F0";
    }

    // Report data structure
    public class ReportData
    {
        public List<ReportSection> Sections { get; set; } = new();
        public List<ReportTable> Tables { get; set; } = new();
        public List<ReportChart> Charts { get; set; } = new();
        public Dictionary<string, object> Variables { get; set; } = new();
    }

    // Report section
    public class ReportSection
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
        public ReportSectionType Type { get; set; } = ReportSectionType.Text;
    }



    // Report table
    public class ReportTable
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Headers { get; set; } = new();
        public List<List<object>> Rows { get; set; } = new();
        public int Order { get; set; }
        public bool ShowHeaders { get; set; } = true;
        public bool AlternateRowColors { get; set; } = true;
    }

    // Report chart (basic structure for future extensibility)
    public class ReportChart
    {
        public string Title { get; set; } = string.Empty;
        public ChartType Type { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
        public int Order { get; set; }
    }

    // Report generation result
    public class ReportResult
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
        public long FileSizeBytes => Data.Length;
    }
}
