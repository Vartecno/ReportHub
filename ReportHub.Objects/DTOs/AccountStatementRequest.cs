using ReportHub.Objects.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Objects.DTOs
{
    public class AccountStatementRequest
    {
        [Required]
        public ReportFormat Format { get; set; } = ReportFormat.PDF;

        [Required]
        public ReportBrandingDTO Branding { get; set; } = new();

        [Required]
        public AccountStatementDataDTO Data { get; set; } = new();

        public ReportConfigurationDTO? Configuration { get; set; }
    }
}
