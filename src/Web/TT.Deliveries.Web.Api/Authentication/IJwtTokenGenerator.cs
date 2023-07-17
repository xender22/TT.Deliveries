namespace TT.Deliveries.Web.Api.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string role);
}