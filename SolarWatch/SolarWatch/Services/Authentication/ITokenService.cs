using Microsoft.AspNetCore.Identity;

namespace WeatherApi.Services.Authentication;

public interface ITokenService
{
    public string CreateToken(IdentityUser user, string role);
}