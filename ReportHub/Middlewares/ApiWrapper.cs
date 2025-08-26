using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace ReportHub.Middlewares
{
    public class ApiWrapper
    {
        /// <summary>
        /// API version class member
        /// </summary>
        //private string ApiVersion;

        /// <summary>
        /// API name class member
        /// </summary>
        //private string ApiName;

        /// <summary>
        /// Request processing task member
        /// </summary>
        private readonly RequestDelegate Next;

        //private IApiLogService _IApiLogService;
        //private IWorkContext _workContext;
        /// <summary>
        /// Initializes a new JsonApiWrapper Middle-ware instance.
        /// </summary>
        /// <param name="version">API version to be shown in all responses</param>
        /// <param name="name">API name to be shown in all responses</param>
        /// <param name="requestDelegate">A task that represents the completion of request processing</param>
        /// 

        //public ApiWrapper(string version, string name
        //                    , RequestDelegate requestDelegate
        //    /*, IApiLogService IApiLogService*/
        //    )
        //{
        //    this.ApiVersion = version;
        //    this.ApiName = name;
        //    this.Next = requestDelegate;
        //    //  _IApiLogService = IApiLogService;
        //    // _workContext = workContext;
        //}

        public ApiWrapper(RequestDelegate requestDelegate)
        {

            this.Next = requestDelegate;

            //  _IApiLogService = IApiLogService;
            // _workContext = workContext;
        }

        /// <summary>
        /// Response post-processing handler.
        /// </summary>
        /// <param name="context">HttpContext object</param>
        /// <returns></returns>
        //public async Task Invoke(HttpContext context, IApiLogService IApiLogService, IWorkContext workContext)
        public async Task Invoke(HttpContext context)//   ,IWorkContext workContext)
        {


            //var UserName = context.User.Claims.Where(x => x.Type == "Username").Select(x => x.Value).FirstOrDefault();
            //var SessionId = context.User.Claims.Where(x => x.Type == "SessionId").Select(x => x.Value).FirstOrDefault();

            //_IApiLogService = IApiLogService;
            // _workContext = workContext;

            //First, get the incoming request
            //var siteid =  await _workContext.GetCurrentSiteId();
            //var userid =  await _workContext.GetCurrentUserId();
            var request = await FormatRequest(context.Request);

            //  ApiLogVM logobj = new ApiLogVM();
            //logobj.ApiUrl = context.Request.Path.Value;
            //logobj.FullUrl = context.Request.Host.Value;
            //
            //logobj.Request = request;
            //logobj.HttpMethod = context.Request.Method;
            //logobj.SiteId = siteid == 0 ? null : siteid;
            //logobj.UserId = userid == 0 ? null : userid;

            Stream responseBody = context.Response.Body;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                await this.Next(context);
                memoryStream.Position = 0;
                memoryStream.Seek(0, SeekOrigin.Begin); //seek begin


                //string responseString = new StreamReader(memoryStream).ReadToEnd();Encoding.UTF8
                string jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());
                string wrappedResponse = this.Wrap(jsonString, context);
                byte[] responseBytes = Encoding.UTF8.GetBytes(wrappedResponse);
                //logobj.Response = wrappedResponse;
                //logobj.StatusCode = context.Response.StatusCode;
                context.Response.Headers["Content-type"] = "application/vnd.api+json";
                context.Response.Headers.Remove("Content-Length");
                context.Response.Body = responseBody;
                // await ExecuteCreateLog(logobj);
                //await _IApiLogService.CreateLog(logobj);
                await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }

        //private async Task<bool> ExecuteCreateLog(ApiLogVM log)
        //{
        //    try
        //    {
        //        Task.Run(async () => await _IApiLogService.CreateLog(log)).ConfigureAwait(false);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}


        private static async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;

            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length)).ConfigureAwait(false);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            // reset the stream position to 0, which is allowed because of EnableBuffering()
            request.Body.Seek(0, SeekOrigin.Begin);

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }
        /// <summary>
        /// Wraps body content into json:api specification.
        /// </summary>
        /// <param name="originalBody">Original body content</param>
        /// <param name="context">HttpContext object</param>
        /// <returns>Wrapped JSON string</returns>
        private dynamic Wrap(string originalBody, HttpContext context)
        {
            dynamic response;
            if (originalBody.StartsWith("{") && originalBody.EndsWith("}"))
            {
                response = JObject.Parse(originalBody);
            }
            else if (originalBody.StartsWith("[") && originalBody.EndsWith("]"))
            {
                response = JArray.Parse(originalBody);
            }
            else if (originalBody == "" && context.Response.StatusCode == 204)
            {
                context.Response.StatusCode = 200;
                response = originalBody;
            }
            else
            {
                response = originalBody;
            }

            object wrapper;
            if (this.IsSuccessResponse(context.Response.StatusCode))
                wrapper = this.DataWrap(response);
            else
                wrapper = this.ErrorWrap(response.ToString(), context);

            string newBody = JsonConvert.SerializeObject(wrapper, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                    {
                        ProcessDictionaryKeys = true
                    }
                }
            });

            return newBody;
        }

        /// <summary>
        /// Retrieves api:json's api root object
        /// </summary>
        /// <returns>API object info</returns>
        //private Object ApiInfo()
        //{
        //    return new
        //    {
        //        version = this.ApiVersion,
        //        name = this.ApiName
        //    };
        //}

        /// <summary>
        /// Rewrites original body content for success wrapping.
        /// </summary>
        /// <param name="response">Response body</param>
        /// <returns>Formatted object</returns>
        private Object DataWrap(dynamic response)
        {
            return new
            {
                code = "200",
                message = "Ok",
                content = response
            };
        }

        /// <summary>
        /// Rewrites original body content for error wrapping.
        /// </summary>
        /// <param name="response">Response body</param>
        /// <returns>Formatted object</returns>
        private Object ErrorWrap(string response, HttpContext context)
        {
            string reason = String.IsNullOrWhiteSpace(response) ? this.StatusCodeMessage(context.Response.StatusCode) : response;
            string CustomCode = string.Empty;
            string CustomMessage = string.Empty;
            try
            {
                var GetError = response.Split('$');
                if (GetError.Length > 1)
                {
                    CustomCode = GetError[0].ToString();
                    CustomMessage = GetError[1].ToString();
                }

                if (response.Contains("(401) Unauthorized"))
                {
                    CustomCode = "401";
                }
            }
            catch (Exception)
            {

            }


            return new
            {
                code = string.IsNullOrEmpty(CustomCode) ? context.Response.StatusCode.ToString() : CustomCode,
                message = string.IsNullOrEmpty(CustomMessage) ? reason : CustomMessage,

            };
        }

        /// <summary>
        /// Determine whether a response is a success or not by its status code.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private Boolean IsSuccessResponse(int statusCode)
        {
            return (statusCode >= 200 && statusCode < 299);
        }

        private string StatusCodeMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "Bad request.";
                case 401:
                    return "Unauthorized access.";
                case 402:
                    return "Payment required.";
                case 403:
                    return "Forbidden access.";
                case 404:
                    return "Resource not found.";
                case 405:
                    return "Method not allowed.";
                case 406:
                    return "Not acceptable.";
                case 407:
                    return "Proxy authentication required.";
                case 408:
                    return "Request timeout.";
                case 409:
                    return "Conflict";
                case 410:
                    return "Resource is gone.";
                case 411:
                    return "Length is required.";
                case 500:
                    return "Internal server error.";
                case 501:
                    return "Not implemented.";
                case 502:
                    return "Bad gateway.";
                case 503:
                    return "Service unavailable.";
                case 504:
                    return "Gateway timeout.";
                case 505:
                    return "HTTP version not supported.";
            }
            return "";
        }


    }

}
