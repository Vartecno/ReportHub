 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReportHub.Common.DataHelpers.IDataHelpers;
using ReportHub.Objects.Enum;


namespace ReportHub.Common.DataHelpers;


public class LoggerHelper : ILoggerHelper
{
    private readonly ILogger _logger;
 //   private LogisticsSalesContext _context;
    public LoggerHelper(ILogger<LoggerHelper> logger
       // , LogisticsSalesContext context)
       )
    {
        _logger = logger;
      //  _context = context;

    }
    public async Task AddLog(HttpContext _HttpContext, dynamic Obj = null)
    {
        try
        {


            //  LOG_WebLogs logger = new LOG_WebLogs();
            //logger.CreationDate = DateTime.UtcNow;
            //logger.ID = Guid.NewGuid();
            ////logger.ac = _HttpContext.Request.Path.Value;
            //logger.UserID = _HttpContext.Items["userID"] as string;
            //if (_HttpContext.Request.Method.Equals("Get", StringComparison.OrdinalIgnoreCase))
            //{
            //    logger.Params = _HttpContext.Request.Path.Value;

            //}
            //else
            //{
            //    logger.Params = JsonConvert.SerializeObject(Obj);
            //}

            //logger.Token = _HttpContext.Items["Token"] as string;
            //logger.HttpRequestType = (int)Enum.Parse(typeof(HttpRequestTypes), _HttpContext.Request.Method);
            //logger.Controller = _HttpContext.Request.Path.ToString();
            //await _context.LOG_WebLogs.AddAsync(logger);
            //await _context.SaveChangesAsync();
            _logger.LogInformation("DB Connection Lost when try to call Api :" + _HttpContext.Request.Path.Value);

        }
        catch (Exception)
        {
            _logger.LogInformation("DB Connection Lost when try to call Api :" + _HttpContext.Request.Path.Value);
        }
    }
}
