using CloudinaryDotNet;
using ReportHub.Objects.DTOs;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ReportHub.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IConfiguration _configuration;
        private string ERPBaseUrl;  
        public TokenMiddleware(RequestDelegate next, HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _next = next;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            ERPBaseUrl = _configuration.GetSection("APIs:ERP:BaseURL").Value;
        }

        public Task Invoke(HttpContext httpContext)
        {
            try
            {
                string token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(token);
                    var data = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "Data").Value;
                    var DecryptedData = this.Decrypt(data);
                    httpContext.Items["Token"] = token;
                    var TokenData = JsonConvert.DeserializeObject<ClaimsList>(DecryptedData);
                    httpContext.Items["Data"] = TokenData;
                    httpContext.Items["CompanyID"] = TokenData.CompanyId;
                    httpContext.Items["UserID"] = TokenData.UserID;
                    httpContext.Items["BranchID"] = TokenData.BranchId;
                    var PermissionFromCookie = _httpContextAccessor.HttpContext.Request.Cookies["Permissions"];
                    if (string.IsNullOrEmpty(PermissionFromCookie))
                    {

                        var ApiURl = _configuration.GetSection("APIs:ERP:ERPAPI:PermissionFromCookie").Value;
                        string Url = ERPBaseUrl+ ApiURl + $"?userId={TokenData.UserID}&ModulesId=10&companyId={TokenData.CompanyId}&branchId={TokenData.BranchId}";
                        //string Url = $"https://coreapi.vartecks.com/Users/GetUserPermission?userId={TokenData.UserID}&ModulesId=10&companyId={TokenData.CompanyId}&branchId={TokenData.BranchId}";
                        
                        HttpResponseMessage response = _httpClient.GetAsync(Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                JsonDocument doc = JsonDocument.Parse(responseBody);
                                string permissions = doc.RootElement
                                    .GetProperty("ResponseDetails")
                                    .GetProperty("Permissions")
                                    .GetString();
                                var CookieValue = permissions;
                                httpContext.Response.Cookies.Append("Permissions", CookieValue, new CookieOptions
                                {
                                    Expires = DateTimeOffset.UtcNow.AddDays(1),
                                    HttpOnly = true // Optional: To make cookie accessible only via HTTP request (not JavaScript)
                                });
                                httpContext.Items["Permissions"] = permissions;
                            }
                            else
                            {
                                httpContext.Items["Permissions"] = "";
                            }

                        }
                        else
                        {
                            httpContext.Items["Permissions"] = "";
                        }
                    }
                    else
                    {
                        httpContext.Items["Permissions"] = PermissionFromCookie;
                    }

                }
                else
                    httpContext.Items["Token"] = "";
                return _next(httpContext);
            }
            catch
            {
                throw;
            }

        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "ERPCORMAKV2SPBNI99212199998547";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }

}
