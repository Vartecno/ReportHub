using Microsoft.AspNetCore.Http;

namespace ReportHub.Common.DataHelpers.IDataHelpers;

public interface ILoggerHelper
{
    Task AddLog(HttpContext _HttpContext, dynamic Obj = null);

}
