using ReportHub.Objects.DTOs;
using ReportHub.Objects.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Common.Helper.DataHelpers.IDataHelpers
{
    public interface IPDFHelper:IReportHelper   
    {
        Task<byte[]> GenerateInvoiceAsync(InvoiceResponseDTO invoice, CompanyInfo companyInfo);
    }
}
