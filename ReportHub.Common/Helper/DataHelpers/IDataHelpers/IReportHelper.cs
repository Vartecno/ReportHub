using ReportHub.Objects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHub.Common.Helper.DataHelpers.IDataHelpers
{
     public interface IReportHelper
    {
        string GetContentType();
        string GetFileExtension();
    }
}
