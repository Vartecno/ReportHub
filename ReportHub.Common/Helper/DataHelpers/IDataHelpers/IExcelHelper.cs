using ReportHub.Objects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Common.Helper.DataHelpers.IDataHelpers
{
    public interface IExcelHelper:IReportHelper
    {
        Task<byte[]> GenerateProfitReportAsync(List<OrderProfitResultDTO> profits, DateTime fromDate, DateTime toDate);
        Task<byte[]> GenerateOrderDetailsReportAsync(List<OrderLeadDetailsDTO> orderDetails);
    }
}
