using JWTAuth.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    { 
        _tokenService = tokenService;
    }

    [HttpGet("login/{username}")]
    public IActionResult Login(string username)
    {
        var token = _tokenService.GenerateToken(username);
        return Ok(token);
    }
}