using System.Security.Claims;
using JWTAuth.Models;
using JWTAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(TokenService tokenService, TokenRepository tokenRepository) : ControllerBase
{
    private static List<User> _users = new List<User>();
    private readonly TokenService _tokenService = tokenService;
    private readonly TokenRepository _tokenRepository = tokenRepository;

    [HttpGet("register/{username}/{password}")]
    public IActionResult Register(string username, string password)
    {
        if (_users.Any(u => u.Name == username))
        {
            return Unauthorized();
        }
        
        _users.Add(new User(username, password));
        var accessToken = _tokenService.GenerateAccessToken(username);
        var refreshToken = _tokenService.GenerateRefreshToken(username);
        
        _tokenRepository.SaveRefreshToken(new RefreshToken
        {
            Token = refreshToken,
            Username = username,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        return Ok(new {
            accessToken, refreshToken
        });
    }
    
    [HttpGet("login/{username}/{password}")]
    public IActionResult Login(string username, string password)
    {
        User? user = _users.FirstOrDefault(u => u.Name == username);
        
        _tokenRepository.RemoveRefreshTokenByName(username);
        
        if (user == null || user.Password != password)
        {
            return Unauthorized();
        }
        
        var accessToken = _tokenService.GenerateAccessToken(username);
        var refreshToken = _tokenService.GenerateRefreshToken(username);
        
        _tokenRepository.SaveRefreshToken(new RefreshToken
        {
            Token = refreshToken,
            Username = username,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        
        return Ok(new {
            accessToken, refreshToken
        });
    }
    
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] string token)
    {
        var refreshToken = _tokenRepository.GetRefreshToken(token);

        if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        _tokenRepository.RemoveRefreshToken(token);

        var newAccessToken = _tokenService.GenerateAccessToken(refreshToken.Username);
        var newRefreshToken = _tokenService.GenerateRefreshToken(refreshToken.Username);

        _tokenRepository.SaveRefreshToken(new RefreshToken
        {
            Token = newRefreshToken,
            Username = refreshToken.Username,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [Authorize]
    [HttpGet("getme/")]
    public IActionResult GetMe()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized("User not found in token.");
        }
        
        User? user = _users.FirstOrDefault(u => u.Name == username);
        return Ok(user);
    }
}