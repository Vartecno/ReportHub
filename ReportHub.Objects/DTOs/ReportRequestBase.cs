using ReportHub.Objects.Enum;
using ReportHub.Objects.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Objects.DTOs
{
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
