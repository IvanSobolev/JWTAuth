using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuth.Models;

public static class AuthOptions
{
    public const string ISSUER = "DOMAIN";
    public const string AUDIENCE = "CLIENT";
    const string KEY = "mysupersecret_secretsecretsecretkey!123";

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
        
}