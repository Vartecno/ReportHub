using System.Security.Claims;

namespace ReportHub.Common.Authentication;

public interface IJwtProvider
{
    Task<(string accessToken, string refreshToken)> GenerateAsync(int userId, List<int> roles);
    Task<ClaimsPrincipal> ValidateRefreshTokenAsync(string refreshToken);
    Dictionary<string, string> GetTokenInfo(string token);
    string GetTokenUserID(string token);
    Task<bool> GetValidTokenAsync(string jwtToken);

}
