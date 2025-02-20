namespace JWTAuth.Models;

public class TokenRepository
{
    private List<RefreshToken?> _refreshTokens = new List<RefreshToken?>();
    
    public void SaveRefreshToken(RefreshToken? refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }

    public RefreshToken? GetRefreshToken(string token)
    {
        return _refreshTokens.FirstOrDefault(rt => rt.Token == token);
    }

    public void RemoveRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
        if (refreshToken != null)
        {
            _refreshTokens.Remove(refreshToken);
        }
    }
    
    public void RemoveRefreshTokenByName(string username)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Username == username);
        if (refreshToken != null)
        {
            _refreshTokens.Remove(refreshToken);
        }
    }
}