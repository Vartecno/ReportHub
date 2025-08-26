using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReportHub.Common.Authentication
{
    public sealed class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;
        public readonly IConfiguration _configuration;
        private string TokenKey;
        private string RefreshTokenKey;
        private int RefreshTokenExpirationMinutes;

        public JwtProvider(IOptions<JwtOptions> options
            , IConfiguration Configuration)
        {
            _options = options.Value;
            _configuration = Configuration;
            TokenKey = _configuration.GetSection("AppSettings:SecretKey").Value;
            RefreshTokenKey = _configuration.GetSection("AppSettings:RefreshTokenSecretKey").Value;
            RefreshTokenExpirationMinutes = Convert.ToInt32(_configuration.GetSection("AppSettings:RefreshTokenExpirationMinutes").Value);

        }

        public async Task<(string accessToken, string refreshToken)> GenerateAsync(int userId, List<int> roles)
        {
            try
            {
                var claims = new List<Claim>
            {
                new Claim("UserID", userId.ToString())
            };
                claims.Add(new Claim("GroupIDs", JsonConvert.SerializeObject(roles)));
                var accessToken = await GenerateAccessToken(claims);
                var refreshToken = await GenerateRefreshTokenAsync();

                return (accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<string> GenerateAccessToken(List<Claim> claims)
        {
            try
            {
                var credentials = new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey)),
                            SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: credentials);

                return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            try
            {

                var refreshTokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RefreshTokenKey));
                var refreshTokenDescriptor = new SecurityTokenDescriptor
                {
                    Expires = DateTime.UtcNow.AddMinutes(RefreshTokenExpirationMinutes),
                    SigningCredentials = new SigningCredentials(refreshTokenKey, SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);

                return await Task.FromResult(tokenHandler.WriteToken(refreshToken));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<bool> GetValidTokenAsync(string jwtToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey))
                };

                SecurityToken validatedToken;
                var principal = await tokenHandler.ValidateTokenAsync(jwtToken, validationParameters);

                validatedToken = principal?.SecurityToken;

                var jwtTokenValid = (validatedToken != null) && (validatedToken.ValidTo > DateTime.UtcNow);
                return jwtTokenValid;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }
        public async Task<ClaimsPrincipal> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var refreshTokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RefreshTokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

                try
                {
                    var principal = tokenHandler.ValidateToken(refreshToken, refreshTokenValidationParameters, out _);
                    return await Task.FromResult(principal);
                }
                catch
                {
                    return await Task.FromResult<ClaimsPrincipal>(null);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Dictionary<string, string> GetTokenInfo(string token)
        {
            try
            {
                var tokenInfo = new Dictionary<string, string>();
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                var claims = jwtSecurityToken.Claims;

                foreach (var claim in claims)
                {
                    tokenInfo.Add(claim.Type, claim.Value);
                }
                return tokenInfo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetTokenUserID(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                var claim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "UserID");
                return claim?.Value;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }

}
