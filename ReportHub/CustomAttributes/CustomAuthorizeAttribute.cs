
using ReportHub.Common.Authentication;
using ReportHub.Objects.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ReportHub.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : Attribute, IAsyncActionFilter
    {

        private int[] PermissionID;
        public CustomAuthorizeAttribute(int[] permissionID)
        {
            PermissionID = permissionID;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var token = context.HttpContext.Items["Token"] as string;
                if (string.IsNullOrEmpty(token))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    await context.HttpContext.Response.WriteAsync("Unauthorized client");
                }
                else
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(token);

                    var jwtProvider = (IJwtProvider)context.HttpContext.RequestServices.GetService(typeof(IJwtProvider));
                    if (!await jwtProvider.GetValidTokenAsync(token))
                    {
                        context.HttpContext.Response.StatusCode = 401;
                        await context.HttpContext.Response.WriteAsync("Unauthorized client");
                    }
                    else
                    {
                        var Permissions = context.HttpContext.Items["Permissions"] as string;
                        if (!string.IsNullOrEmpty(Permissions))
                        {
                            var result = Permissions.Split(',').Select(int.Parse).ToArray();
                            if (!result.Any(id => PermissionID.Contains(id)))
                            {
                                context.HttpContext.Response.StatusCode = 403;
                                await context.HttpContext.Response.WriteAsync("Forbidden client");
                                return;
                            }
                        }
                        await next();

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }

}
